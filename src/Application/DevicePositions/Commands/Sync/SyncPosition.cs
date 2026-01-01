// Copyright (c) 2025 Sergio Hernandez. All rights reserved.
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

using TrackHubRouter.Application.DevicePositions.Events;

namespace TrackHubRouter.Application.DevicePositions.Commands.Sync;

public readonly record struct SyncPositionCommand() : IRequest<bool>;

public class UpdateTransporterCommandHandler(IAccountReader reader,
    IOperatorReader operatorReader,
    IExecutionIntervalManager intervalManager,
    IPublisher publisher) : IRequestHandler<SyncPositionCommand, bool>
{

    public async Task<bool> Handle(SyncPositionCommand request, CancellationToken cancellationToken)
    { 
        var accounts = await reader.GetAccountsToSyncAsync(cancellationToken);
        foreach (var account in accounts)
        {
            if (intervalManager.ShouldExecuteTask(account))
            {
                var operators = await operatorReader.GetOperatorsByAccountsAsync(account.AccountId, cancellationToken);
                foreach (var @operator in operators)
                {
                    var operatorCredential = await operatorReader.GetOperatorAsync(@operator.OperatorId, cancellationToken);
                    await publisher.Publish(new OperatorRetrieved.Notification(operatorCredential), cancellationToken);
                }
                intervalManager.UpdateLastExecutionTime(account.AccountId);
            }
        }
        return true;
    }
}
