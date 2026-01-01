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

using Ardalis.GuardClauses;
using Common.Application.Attributes;
using Common.Domain.Constants;
using Microsoft.Extensions.Configuration;
using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.PingOperator.Queries;

[Authorize(Resource = Resources.Credentials, Action = Actions.Custom)]
public readonly record struct PingOperatorQuery(Guid OperatorId) : IRequest<bool>;

// This class handles the PingOperatorQuery and implements the IRequestHandler interface
public class PingOperatorQueryHandler(
    IConfiguration configuration,
    IOperatorReader operatorReader,
    IConnectivityRegistry connectivityRegistry)
    : IRequestHandler<PingOperatorQuery, bool>
{
    // This property retrieves the EncryptionKey from the configuration
    private string? EncryptionKey { get; } = configuration["AppSettings:EncryptionKey"];

    // This method handles the PingOperatorQuery and returns a boolean indicating the success of the operation
    public async Task<bool> Handle(PingOperatorQuery request, CancellationToken cancellationToken)
    {
        // Get the operator details using the operatorReader
        var @operator = await operatorReader.GetOperatorAsync(request.OperatorId, cancellationToken);
        // Ping the operator asynchronously
        return await PingAsync(@operator, cancellationToken);
    }

    // This method pings the operator by testing the connectivity
    private async Task<bool> PingAsync(
        OperatorVm @operator,
        CancellationToken cancellationToken)
    {
        // Get the connectivity tester based on the protocol type of the operator
        var reader = connectivityRegistry.GetTester((ProtocolType)@operator.ProtocolTypeId);
        // Test the connectivity asynchronously
        return await TestConnectivityAsync(reader, @operator, cancellationToken);
    }

    // This method tests the connectivity by pinging the operator
    private async Task<bool> TestConnectivityAsync(
        IConnectivityTester reader,
        OperatorVm @operator,
        CancellationToken cancellationToken)
    {
        // Ensure that the EncryptionKey is not null, otherwise throw an exception
        Guard.Against.Null(EncryptionKey, message: "Credential key not found.");
        // If the operator has a credential, ping using the decrypted credential value
        if (@operator.Credential is not null)
        {
            await reader.Ping(@operator.Credential.Value.Decrypt(EncryptionKey), cancellationToken);
            return true;
        }
        return false;
    }
}
