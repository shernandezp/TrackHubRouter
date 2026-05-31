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

            private static bool IsValidPosition(PositionVm position)
                => position.TransporterId != Guid.Empty
                   && position.DeviceDateTime != default
                   && position.Latitude is >= -90d and <= 90d
                   && position.Longitude is >= -180d and <= 180d;
        }
    }
}
