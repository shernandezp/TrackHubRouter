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
using TrackHubRouter.Application.Positions.Mappers;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.Positions.Queries.GetTrips;

[Authorize(Resource = Resources.Positions, Action = Actions.Read)]
public readonly record struct GetPositionTripsQuery(Guid TransporterId, DateTimeOffset From, DateTimeOffset To) : IRequest<IEnumerable<TripVm>>;

public class GetPositionTripsQueryHandler(
        IConfiguration configuration,
        IOperatorReader operatorReader,
        IPositionRegistry positionRegistry,
        IDeviceTransporterReader deviceReader)
        : PositionBaseHandler, IRequestHandler<GetPositionTripsQuery, IEnumerable<TripVm>>
{
    private string? EncryptionKey { get; } = configuration["AppSettings:EncryptionKey"];

    /// <summary>
    /// Retrieves and position trips asynchronously
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the collection of TripVm</returns>
    public async Task<IEnumerable<TripVm>> Handle(GetPositionTripsQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(EncryptionKey, message: "Credential key not found.");
        var @operator = await operatorReader.GetOperatorByTransporterAsync(request.TransporterId, cancellationToken);
        var positions = await GetDevicePositionAsync(
            positionRegistry,
            deviceReader,
            EncryptionKey,
            @operator, 
            request.From, 
            request.To, 
            request.TransporterId, 
            cancellationToken);
        //TODO: Get distance and time threshold from transporter type
        return positions.GroupPositionsIntoTrips(1, TimeSpan.FromMinutes(30));

    }

}
