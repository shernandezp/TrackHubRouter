﻿using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;
using Common.Application.Attributes;
using Common.Domain.Constants;
using Microsoft.Extensions.Configuration;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Extensions;

namespace TrackHubRouter.Application.Positions.Get;

[Authorize(Resource = Resources.Positions, Action = Actions.Read)]
public readonly record struct GetPositionsQuery() : IRequest<IEnumerable<PositionVm>>;

public class GetPositionsQueryHandler(
        IConfiguration configuration,
        IOperatorReader operatorReader,
        IPositionRegistry positionRegistry,
        IDeviceReader deviceReader)
        : IRequestHandler<GetPositionsQuery, IEnumerable<PositionVm>>
{
    private string? EncryptionKey { get; } = configuration["AppSettings:EncryptionKey"];

    /// <summary>
    /// Retrieves the operators, protocols, and device positions asynchronously
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the collection of PositionVm</returns>
    public async Task<IEnumerable<PositionVm>> Handle(GetPositionsQuery request, CancellationToken cancellationToken)
    {
        var operators = await operatorReader.GetOperatorsAsync(cancellationToken);
        var protocols = operators.Select(o => (ProtocolType)o.ProtocolTypeId).Distinct();

        var allPositions = new List<PositionVm>();
        await foreach (var positionsCollection in GetDevicePositionAsync(operators, protocols, cancellationToken))
        {
            allPositions.AddRange(positionsCollection);
        }

        return allPositions;
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
        Guard.Against.Null(EncryptionKey, message: "Credential key not found.");
        var @operator = operators.FirstOrDefault(o => (ProtocolType)o.ProtocolTypeId == reader.Protocol);
        if (@operator.Credential is not null)
        {
            await reader.Init(@operator.Credential.Value.Decrypt(EncryptionKey), cancellationToken);
            var devices = await deviceReader.GetDevicesByOperatorAsync(@operator.OperatorId, cancellationToken);
            try
            {
                return await reader.GetDevicePositionAsync(devices, cancellationToken);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                //go to the local db, this in case the api is down
                return [];
            }
        }
        return [];
    }
}
