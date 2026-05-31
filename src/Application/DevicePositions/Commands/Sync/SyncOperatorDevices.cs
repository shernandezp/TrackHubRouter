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

using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.DevicePositions.Commands.Sync;

public readonly record struct SyncOperatorDevicesCommand(
    OperatorVm Operator,
    AccountSettingsVm Account,
    string TriggerType,
    string? CorrelationId = null,
    bool ResetDeviceCatalog = false,
    bool AutoAssignNewDevices = true) : IRequest<bool>;

public class SyncOperatorDevicesCommandHandler(
    IConfiguration configuration,
    IDeviceRegistry deviceRegistry,
    IDeviceSyncWriter deviceSyncWriter,
    IOperatorSyncRunWriter syncRunWriter,
    IAlertEventWriter alertWriter,
    ILogger<SyncOperatorDevicesCommandHandler> logger) : IRequestHandler<SyncOperatorDevicesCommand, bool>
{
    private string? EncryptionKey { get; } = configuration["AppSettings:EncryptionKey"];

    public async Task<bool> Handle(SyncOperatorDevicesCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(EncryptionKey, message: "Credential key not found.");
        if (request.Operator.Credential is null)
        {
            return false;
        }

        var startedAt = DateTimeOffset.UtcNow;
        var correlationId = request.CorrelationId ?? Guid.NewGuid().ToString();
        var result = "SUCCEEDED";
        string? errorCode = null;
        string? errorMessage = null;
        var devices = Array.Empty<DeviceVm>();

        try
        {
            var reader = deviceRegistry.GetReader((ProtocolType)request.Operator.ProtocolTypeId);
            await reader.Init(request.Operator.Credential.Value.Decrypt(EncryptionKey), cancellationToken);
            devices = (await reader.GetDevicesAsync(cancellationToken))?.ToArray() ?? [];
            if (request.ResetDeviceCatalog)
            {
                await deviceSyncWriter.ResetAsync(request.Account.AccountId, request.Operator.OperatorId, cancellationToken);
            }

            var dtos = devices.Select(d => new SynchronizedDeviceDto(
                AccountId: request.Account.AccountId,
                OperatorId: request.Operator.OperatorId,
                Serial: d.Serial,
                Name: d.Name,
                Identifier: d.Identifier,
                ProviderDisplayName: d.ProviderDisplayName ?? d.Name,
                DeviceTypeId: d.DeviceTypeId,
                Description: null,
                ProviderMetadataHash: d.ProviderMetadataHash,
                ProviderStatus: d.ProviderStatus));

            await deviceSyncWriter.SynchronizeAsync(
                request.Account.AccountId,
                request.Operator.OperatorId,
                dtos,
                correlationId,
                request.TriggerType,
                request.AutoAssignNewDevices,
                cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            result = "FAILED";
            errorCode = ex.GetType().Name;
            errorMessage = ex.Message;
            logger.LogError(ex, "Device sync failed for operator {OperatorId} (account {AccountId}).",
                request.Operator.OperatorId, request.Account.AccountId);
        }

        try
        {
            await syncRunWriter.RecordAsync(new OperatorSyncRunDto(
                AccountId: request.Account.AccountId,
                OperatorId: request.Operator.OperatorId,
                TriggerType: request.TriggerType,
                Result: result,
                StartedAt: startedAt,
                CompletedAt: DateTimeOffset.UtcNow,
                DevicesSeen: devices.Length,
                DevicesAdded: 0,
                DevicesUpdated: 0,
                DevicesRemoved: 0,
                DevicesIgnored: 0,
                PositionsRead: 0,
                PositionsAccepted: 0,
                PositionsRejected: 0,
                ErrorCode: errorCode,
                ErrorMessage: errorMessage,
                CorrelationId: correlationId), cancellationToken);

            if (result == "FAILED")
            {
                await alertWriter.RecordAsync(new AlertEventDto(
                    AccountId: request.Account.AccountId,
                    EventType: "GpsOperatorDeviceSyncFailed",
                    Severity: "Warning",
                    SourceModule: "TrackHubRouter.SyncWorker",
                    ResourceType: "Operator",
                    ResourceId: request.Operator.OperatorId.ToString(),
                    Status: "Open",
                    PayloadJson: System.Text.Json.JsonSerializer.Serialize(new { errorCode, message = errorMessage ?? "Device sync failed" }),
                    DeduplicationKey: $"device-sync-failed:{request.Operator.OperatorId}:{DateTimeOffset.UtcNow:yyyyMMddHH}"), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to record device-sync telemetry for operator {OperatorId}.",
                request.Operator.OperatorId);
        }

        return false;
    }
}
