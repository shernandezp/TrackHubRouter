// Copyright (c) 2026 Sergio Hernandez. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using Microsoft.Extensions.Logging;
using TrackHubRouter.Domain.Interfaces.Geocoding;
using TrackHubRouter.Domain.Interfaces.Geofence;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.DevicePositions.Events;

public sealed class PositionsRetrieved
{
    public readonly record struct Notification(
        IEnumerable<PositionVm> Positions,
        AccountSettingsVm Settings,
        OperatorVm Operator,
        DateTimeOffset StartedAt,
        string TriggerType,
        string CorrelationId) : INotification
    {
        public class EventHandler(
            IPositionWriter positionWriter,
            IGeofenceWriter geofenceWriter,
            IOperatorSyncRunWriter syncRunWriter,
            IAlertEventWriter alertWriter,
            IReverseGeocodingService geocodingService,
            IPositionHistorySystemWriter historyWriter,
            ILogger<EventHandler> logger) : INotificationHandler<Notification>
        {
            public async Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                var positions = notification.Positions?.ToArray() ?? [];
                var positionsRead = positions.Length;
                int positionsAccepted = 0;
                string? errorCode = null;
                string? errorMessage = null;
                var result = "SUCCEEDED";

                var validPositionCandidates = positions
                    .Where(IsValidPosition)
                    .ToArray();
                var invalidPositionCount = positionsRead - validPositionCandidates.Length;

                // Bounded address enrichment: resolve addresses for rows the GPS provider
                // left blank, up to the per-cycle budget, honoring the active provider's
                // throttle. Failures are logged and never block or delay position storage.
                // Enrich the full set of valid fixes so both the latest projection and the stored
                // history carry addresses.
                validPositionCandidates = await EnrichAddressesAsync(validPositionCandidates, cancellationToken);

                // The per-transporter dedupe (freshest fix per transporter wins) applies ONLY to the
                // latest-position projection (spec 01.3 A4 / K6). The history append below receives
                // every valid fix — a transporter with two active devices records both — while the
                // idempotency key prevents duplicates.
                var validPositions = validPositionCandidates
                    .GroupBy(p => p.TransporterId)
                    .Select(g => g
                        .OrderByDescending(p => p.DeviceDateTime)
                        .ThenByDescending(p => p.ServerDateTime ?? DateTimeOffset.MinValue)
                        .First())
                    .ToArray();
                positionsAccepted = validPositions.Length;
                if (invalidPositionCount > 0)
                {
                    result = validPositions.Length == 0 ? "FAILED" : "PARTIALLY_SUCCEEDED";
                    errorCode = "InvalidPosition";
                    errorMessage = $"{invalidPositionCount} position(s) rejected by validation.";
                }

                try
                {
                    if (validPositions.Length > 0)
                    {
                        var writeSucceeded = await positionWriter.AddOrUpdatePositionAsync(validPositions, cancellationToken);
                        if (!writeSucceeded)
                        {
                            result = "FAILED";
                            positionsAccepted = 0;
                            errorCode = "PositionWriteRejected";
                            errorMessage = "Manager rejected the position upsert.";
                        }

                        if (writeSucceeded && notification.Settings.GeofencingEnabled)
                        {
                            await geofenceWriter.ProcessPositionsAsync(validPositions, notification.Settings.AccountId, cancellationToken);
                        }

                        // Stored history: the account-level sync cadence already runs at the
                        // configured storing interval, so every accepted batch is appended for
                        // accounts with gps.positionHistory. History receives ALL valid fixes (not the
                        // deduped latest set) so multi-device transporters keep every fix (spec 01.3
                        // A4 / K6). Failures never fail the sync run.
                        if (writeSucceeded && notification.Settings.GpsPositionHistoryEnabled)
                        {
                            try
                            {
                                await historyWriter.AppendRangeAsync(
                                    notification.Settings.AccountId,
                                    notification.Operator.OperatorId,
                                    validPositionCandidates,
                                    cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                logger.LogWarning(ex, "Position history append failed for operator {OperatorId} (account {AccountId}).",
                                    notification.Operator.OperatorId, notification.Settings.AccountId);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = "FAILED";
                    positionsAccepted = 0;
                    errorCode = ex.GetType().Name;
                    errorMessage = ex.Message;
                    logger.LogError(ex, "Position sync failed for operator {OperatorId} (account {AccountId}).",
                        notification.Operator.OperatorId, notification.Settings.AccountId);
                }

                try
                {
                    await syncRunWriter.RecordAsync(new OperatorSyncRunDto(
                        AccountId: notification.Settings.AccountId,
                        OperatorId: notification.Operator.OperatorId,
                        TriggerType: notification.TriggerType,
                        Result: result,
                        StartedAt: notification.StartedAt,
                        CompletedAt: DateTimeOffset.UtcNow,
                        DevicesSeen: 0,
                        DevicesAdded: 0,
                        DevicesUpdated: 0,
                        DevicesRemoved: 0,
                        DevicesIgnored: 0,
                        PositionsRead: positionsRead,
                        PositionsAccepted: positionsAccepted,
                        PositionsRejected: positionsRead - positionsAccepted,
                        ErrorCode: errorCode,
                        ErrorMessage: errorMessage,
                        CorrelationId: notification.CorrelationId), cancellationToken);

                    if (result == "FAILED")
                    {
                        await alertWriter.RecordAsync(new AlertEventDto(
                            AccountId: notification.Settings.AccountId,
                            EventType: "GpsOperatorPositionSyncFailed",
                            Severity: "Warning",
                            SourceModule: "TrackHubRouter.SyncWorker",
                            ResourceType: "Operator",
                            ResourceId: notification.Operator.OperatorId.ToString(),
                            Status: "Open",
                            PayloadJson: System.Text.Json.JsonSerializer.Serialize(new { errorCode, message = errorMessage ?? "Position sync failed" }),
                            DeduplicationKey: $"position-sync-failed:{notification.Operator.OperatorId}:{DateTimeOffset.UtcNow:yyyyMMddHH}"), cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to record sync telemetry for operator {OperatorId}.",
                        notification.Operator.OperatorId);
                }
            }

            private async Task<PositionVm[]> EnrichAddressesAsync(PositionVm[] positions, CancellationToken cancellationToken)
            {
                try
                {
                    var budget = await geocodingService.GetEnrichmentBudgetAsync(cancellationToken);
                    if (budget <= 0)
                    {
                        return positions;
                    }

                    for (var i = 0; i < positions.Length && budget > 0; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(positions[i].Address))
                        {
                            continue;
                        }

                        budget--;
                        var address = await geocodingService.TryResolveAsync(positions[i].Latitude, positions[i].Longitude, cancellationToken);
                        if (address.HasValue && !string.IsNullOrWhiteSpace(address.Value.Address))
                        {
                            positions[i] = positions[i] with
                            {
                                Address = address.Value.Address,
                                City = address.Value.City,
                                State = address.Value.State,
                                Country = address.Value.Country
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Address enrichment failed; storing positions without addresses.");
                }

                return positions;
            }

            private static bool IsValidPosition(PositionVm position)
                => position.TransporterId != Guid.Empty
                   && position.DeviceDateTime != default
                   && position.Latitude is >= -90d and <= 90d
                   && position.Longitude is >= -180d and <= 180d;
        }
    }
}
