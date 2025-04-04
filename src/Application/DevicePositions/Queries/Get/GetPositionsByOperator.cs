﻿using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Application.DevicePositions.Events;

namespace TrackHubRouter.Application.DevicePositions.Queries.Get;

public readonly record struct GetPositionsByOperatorQuery(OperatorVm Operator) : IRequest<bool>;

public class GetPositionsByOperatorQueryHandler(
        IPublisher publisher,
        IConfiguration configuration,
        IPositionRegistry positionRegistry,
        IDeviceTransporterReader deviceReader)
        : IRequestHandler<GetPositionsByOperatorQuery, bool>
{
    private string? EncryptionKey { get; } = configuration["AppSettings:EncryptionKey"];

    /// <summary>
    /// Retrieves the device positions asynchronously
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the collection of PositionVm</returns>
    public async Task<bool> Handle(GetPositionsByOperatorQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(EncryptionKey, message: "Credential key not found.");
        if (request.Operator.Credential is not null)
        {
            var reader = positionRegistry.GetReader((ProtocolType)request.Operator.ProtocolTypeId);
            await reader.Init(request.Operator.Credential.Value.Decrypt(EncryptionKey), cancellationToken);
            var devices = await deviceReader.GetDeviceTransporterAsync(request.Operator.OperatorId, cancellationToken);
            var positions = await TryGetPositionsAsync(reader, devices, cancellationToken);
            if (positions.Any())
            {
                await publisher.Publish(new PositionsRetrieved.Notification(positions), cancellationToken);
            }
        }
        return true;
    }

    private static async Task<IEnumerable<PositionVm>> TryGetPositionsAsync(
        IPositionReader reader,
        IEnumerable<DeviceTransporterVm> devices,
        CancellationToken cancellationToken)
    {
        try
        {
            return await reader.GetDevicePositionAsync(devices, cancellationToken);
        }
        catch
        {
            return [];
        }
    }

}
