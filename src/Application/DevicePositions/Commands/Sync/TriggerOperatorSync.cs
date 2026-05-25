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

namespace TrackHubRouter.Application.DevicePositions.Commands.Sync;

public readonly record struct TriggerOperatorSyncCommand(
    Guid AccountId,
    Guid OperatorId,
    string TriggerType = "MANUAL",
    string? CorrelationId = null) : IRequest<bool>;

public class TriggerOperatorSyncCommandHandler(
    IAccountReader accountReader,
    IOperatorReader operatorReader,
    ISender sender,
    ILogger<TriggerOperatorSyncCommandHandler> logger) : IRequestHandler<TriggerOperatorSyncCommand, bool>
{
    private static readonly TimeSpan ManualSyncCooldown = TimeSpan.FromMinutes(5);

    public async Task<bool> Handle(TriggerOperatorSyncCommand request, CancellationToken cancellationToken)
    {
        var op = await operatorReader.GetOperatorAsync(request.OperatorId, cancellationToken);
        if (op.AccountId != request.AccountId)
        {
            logger.LogWarning("Manual sync trigger rejected: operator {OperatorId} does not belong to account {AccountId}.", request.OperatorId, request.AccountId);
            return false;
        }

        var account = await accountReader.GetAccountToSyncAsync(request.AccountId, cancellationToken);
        if (!account.HasValue)
        {
            logger.LogWarning("Manual sync trigger received for unknown or unauthorized account {AccountId}.", request.AccountId);
            return false;
        }

        if (!account.Value.GpsIntegrationEnabled)
        {
            logger.LogInformation("Manual sync trigger ignored: GPS integration feature disabled for account {AccountId}.", request.AccountId);
            return false;
        }

        if (!op.Enabled)
        {
            logger.LogInformation("Manual sync trigger ignored: operator {OperatorId} is disabled.", request.OperatorId);
            return false;
        }
        if (string.Equals(request.TriggerType, "MANUAL", StringComparison.OrdinalIgnoreCase)
            && op.LastManualSyncAt.HasValue
            && DateTimeOffset.UtcNow - op.LastManualSyncAt.Value < ManualSyncCooldown)
        {
            logger.LogInformation("Manual sync trigger throttled for operator {OperatorId}.", request.OperatorId);
            return false;
        }

        return await sender.Send(new SyncOperatorDevicesCommand(op, account.Value, request.TriggerType, request.CorrelationId), cancellationToken);
    }
}
