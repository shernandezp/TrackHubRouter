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

using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;
using Common.Application.Attributes;
using Common.Domain.Constants;
using Microsoft.Extensions.Configuration;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Application.DevicePositions.Events;

namespace TrackHubRouter.Application.DevicePositions.Queries.Get;

[Authorize(Resource = Resources.Positions, Action = Actions.Read)]
public readonly record struct GetPositionsByUserQuery() : IRequest<IEnumerable<PositionVm>>;

public class GetPositionsByUserQueryHandler(
        IPublisher publisher,
        IConfiguration configuration,
        IOperatorReader operatorReader,
        IPositionRegistry positionRegistry,
        IDeviceTransporterReader deviceReader,
        ITransporterPositionReader transporterPositionReader)
        : IRequestHandler<GetPositionsByUserQuery, IEnumerable<PositionVm>>
{
    private string? EncryptionKey { get; } = configuration["AppSettings:EncryptionKey"];

    /// <summary>
    /// Retrieves the operators, protocols, and device positions asynchronously
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the collection of PositionVm</returns>
    public async Task<IEnumerable<PositionVm>> Handle(GetPositionsByUserQuery request, CancellationToken cancellationToken)
    {
        var operators = await operatorReader.GetOperatorsAsync(cancellationToken);
        var protocols = operators.Select(o => (ProtocolType)o.ProtocolTypeId).Distinct();

        var allPositions = new List<PositionVm>();
        await foreach (var positionsCollection in GetDevicePositionAsync(operators, protocols, cancellationToken))
        {
            allPositions.AddRange(positionsCollection);
        }

        //Most recent position for each transporter if multiple positions are available
        return allPositions
            .GroupBy(p => p.TransporterId)
            .Select(g => g.OrderByDescending(p => p.DeviceDateTime).First())
            .ToList();
    }

    /// <summary>
    /// Retrieves the device positions asynchronously
    /// </summary>
    /// <param name="operators"></param>
    /// <param name="protocols"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>returns the collection of PositionVm</returns>
    private async IAsyncEnumerable<IEnumerable<PositionVm>> GetDevicePositionAsync(
        IEnumerable<OperatorVm> operators,
        IEnumerable<ProtocolType> protocols,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var tasks = positionRegistry.GetReaders(protocols)
            .Select(reader
                => FetchAndProcessPositionsAsync(reader, operators, cancellationToken));
        var fetchTasks = Task.WhenAll(tasks);

        var results = await fetchTasks;
        foreach (var positions in results)
        {
            if (positions.Any())
            {
                yield return positions;
            }
        }
    }

    /// <summary>
    /// Fetches and processes the positions asynchronously
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="operators"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>returns the collection of PositionVm</returns>
    private async Task<IEnumerable<PositionVm>> FetchAndProcessPositionsAsync(
        IPositionReader reader,
        IEnumerable<OperatorVm> operators,
        CancellationToken cancellationToken)
    {
        var @operator = operators.FirstOrDefault(o => (ProtocolType)o.ProtocolTypeId == reader.Protocol);
        return await TryGetPositionsAsync(reader, @operator, cancellationToken);
    }

    private async Task<IEnumerable<PositionVm>> TryGetPositionsAsync(
        IPositionReader reader,
        OperatorVm @operator,
        CancellationToken cancellationToken)
    {
        try
        {
            if (@operator.Credential is null)
                throw new ArgumentNullException(nameof(@operator), "Credential is null");
            Guard.Against.Null(EncryptionKey, message: "Credential key not found.");

            var devices = await deviceReader.GetDevicesByOperatorAsync(@operator.OperatorId, cancellationToken);
            var credential = @operator.Credential.Value.Decrypt(EncryptionKey);
            await reader.Init(credential, cancellationToken);
            var positions = await reader.GetDevicePositionAsync(devices, cancellationToken);
            if (positions.Any())
            {
                await publisher.Publish(new ValidateSync.Notification(@operator.AccountId, positions), cancellationToken);
            }
            return positions;
        }
        catch
        {
            //TODO: Log exception
            return await GetFallbackPositionsAsync(@operator.OperatorId, cancellationToken);
        }
    }

    private async Task<IEnumerable<PositionVm>> GetFallbackPositionsAsync(
        Guid operatorId,
        CancellationToken cancellationToken)
    {
        try
        {
            return await transporterPositionReader.GetTransporterPositionAsync(operatorId, cancellationToken);
        }
        catch
        {
            //TODO: Log exception
            return [];
        }
    }

}
