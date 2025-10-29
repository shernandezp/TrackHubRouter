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
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Extensions;

namespace TrackHubRouter.Application.DevicePositions.Queries.Get;

[Authorize(Resource = Resources.Positions, Action = Actions.Read)]
public readonly record struct GetPositionByTransporterQuery(Guid TransporterId) : IRequest<PositionVm>;

public class GetPositionByTransporterQueryHandler(
        IConfiguration configuration,
        IOperatorReader operatorReader,
        IPositionRegistry positionRegistry,
        IDeviceTransporterReader deviceReader)
        : IRequestHandler<GetPositionByTransporterQuery, PositionVm>
{
    private string? EncryptionKey { get; } = configuration["AppSettings:EncryptionKey"];

    /// <summary>
    /// Get the position of a device by its transporter id.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PositionVm> Handle(GetPositionByTransporterQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(EncryptionKey, message: "Credential key not found.");
        var @operator = await operatorReader.GetOperatorByTransporterAsync(request.TransporterId, cancellationToken);
        var device = await deviceReader.GetDevicesTransporterAsync(request.TransporterId, cancellationToken);
        return await GetDevicePositionAsync(
            EncryptionKey,
            @operator,
            device,
            cancellationToken);
    }

    /// <summary>
    /// Get the position of a device by its transporter id.
    /// </summary>
    /// <param name="encryptionKey"></param>
    /// <param name="operator"></param>
    /// <param name="device"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<PositionVm> GetDevicePositionAsync(
        string encryptionKey,
        OperatorVm @operator,
        DeviceTransporterVm device,
        CancellationToken cancellationToken)
    {
        var reader = positionRegistry.GetReader((ProtocolType)@operator.ProtocolTypeId);
        if (@operator.Credential is not null)
        {
            await reader.Init(@operator.Credential.Value.Decrypt(encryptionKey), cancellationToken);
            var position = await reader.GetDevicePositionAsync(device, cancellationToken);
            return position;
        }
        return default;
    }

}
