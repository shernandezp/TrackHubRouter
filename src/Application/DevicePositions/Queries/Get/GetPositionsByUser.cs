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

using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;
using Common.Application.Attributes;
using Common.Domain.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Extensions;

namespace TrackHubRouter.Application.DevicePositions.Queries.Get;

[Authorize(Resource = Resources.Positions, Action = Actions.Read)]
[RateLimiting(PermitLimit = 3, WindowSeconds = 60)]
public readonly record struct GetPositionsByUserQuery() : IRequest<IEnumerable<PositionVm>>;

public class GetPositionsByUserQueryHandler(
        IConfiguration configuration,
        IAccountReader accountReader,
        IOperatorReader operatorReader,
        IPositionRegistry positionRegistry,
        IDeviceTransporterReader deviceReader,
        ITransporterPositionReader transporterPositionReader,
        ILogger<GetPositionsByUserQueryHandler> logger)
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
        var allOperators = await operatorReader.GetOperatorsAsync(cancellationToken);
        var operators = allOperators.ToList();
        if (operators.Count == 0)
        {
            return [];
        }

        var providerEnabledAccounts = await Application.Gating.GpsFeatureGate.GetProviderIntegrationEnabledAccountIdsAsync(
            accountReader,
            operators.Select(o => o.AccountId),
            cancellationToken);
        var liveOperators = operators
            .Where(o => Application.Gating.GpsFeatureGate.CanReadProviderOnDemand(o, providerEnabledAccounts))
            .ToList();
        var cachedOperators = operators.Except(liveOperators);

        var allPositions = new List<PositionVm>();
        if (liveOperators.Count > 0)
        {
            var protocols = liveOperators.Select(o => (ProtocolType)o.ProtocolTypeId).Distinct();
            await foreach (var positionsCollection in GetDevicePositionAsync(liveOperators, protocols, cancellationToken))
            {
                allPositions.AddRange(positionsCollection);
            }
        }

        foreach (var @operator in cachedOperators)
        {
            allPositions.AddRange(await GetFallbackPositionsAsync(@operator.OperatorId, cancellationToken));
        }

        //Most recent position for each transporter if multiple positions are available
        return [.. allPositions
            .GroupBy(p => p.TransporterId)
            .Select(g => g.OrderByDescending(p => p.DeviceDateTime).First())];
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
            var positionsList = positions.ToList();
            return positionsList.Count > 0
                ? positionsList
                : await GetFallbackPositionsAsync(@operator.OperatorId, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error retrieving positions for user");
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
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error retrieving fallback positions for operator {OperatorId}", operatorId);
            return [];
        }
    }

}
