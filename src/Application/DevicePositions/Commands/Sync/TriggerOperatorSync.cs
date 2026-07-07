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
using TrackHub.Router.Domain.Exceptions;

namespace TrackHub.Router.Application.DevicePositions.Commands.Sync;

public readonly record struct TriggerOperatorSyncCommand(
    Guid AccountId,
    Guid OperatorId,
    string TriggerType = "MANUAL",
    string? CorrelationId = null,
    bool ResetDeviceCatalog = false,
    bool? AutoAssignNewDevices = null) : IRequest<bool>;

public class TriggerOperatorSyncCommandHandler(
    IAccountReader accountReader,
    IOperatorReader operatorReader,
    ISender sender,
    ILogger<TriggerOperatorSyncCommandHandler> logger) : IRequestHandler<TriggerOperatorSyncCommand, bool>
{
    public async Task<bool> Handle(TriggerOperatorSyncCommand request, CancellationToken cancellationToken)
    {
        // The manual-sync throttle lives at the single point of entry — Manager's
        // ManualSyncMinIntervalSeconds, which throws TooManyRequestsException (spec 01.3 A2, K3).
        // The Router's former hardcoded 5-minute cooldown is removed so a trigger accepted by
        // Manager can never be silently dropped here. The Router still validates operator/account/
        // enabled and returns typed errors instead of a silent false.
        var op = await operatorReader.GetOperatorAsync(request.OperatorId, cancellationToken);
        if (op.AccountId != request.AccountId)
        {
            logger.LogWarning("Manual sync trigger rejected: operator {OperatorId} does not belong to account {AccountId}.", request.OperatorId, request.AccountId);
            throw new OperatorNotFoundException(request.OperatorId);
        }

        var account = await accountReader.GetAccountToSyncAsync(request.AccountId, cancellationToken);
        if (!account.HasValue)
        {
            logger.LogWarning("Manual sync trigger received for unknown or unauthorized account {AccountId}.", request.AccountId);
            throw new OperatorNotFoundException(request.OperatorId);
        }

        if (!op.Enabled)
        {
            logger.LogInformation("Manual sync trigger rejected: operator {OperatorId} is disabled.", request.OperatorId);
            throw new OperatorDisabledException(request.OperatorId);
        }

        return await sender.Send(new SyncOperatorDevicesCommand(
            op,
            account.Value,
            request.TriggerType,
            request.CorrelationId,
            request.ResetDeviceCatalog,
            request.AutoAssignNewDevices ?? true), cancellationToken);
    }
}
