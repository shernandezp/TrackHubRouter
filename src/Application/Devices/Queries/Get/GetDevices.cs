using Common.Application.Attributes;
using Common.Domain.Constants;
using System.Runtime.CompilerServices;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Extensions;
using Microsoft.Extensions.Configuration;
using Ardalis.GuardClauses;

namespace TrackHubRouter.Application.Devices.Queries.Get;

[Authorize(Resource = Resources.Devices, Action = Actions.Read)]
public readonly record struct GetDevicesQuery() : IRequest<IEnumerable<DeviceVm>>;

public class GetDevicesQueryHandler(
    IConfiguration configuration,
    IOperatorReader operatorReader,
    IDeviceRegistry deviceRegistry,
    IDeviceTransporterReader deviceReader)
    : IRequestHandler<GetDevicesQuery, IEnumerable<DeviceVm>>
{

    private string? EncryptionKey { get; } = configuration["AppSettings:EncryptionKey"];

    public async Task<IEnumerable<DeviceVm>> Handle(GetDevicesQuery request, CancellationToken cancellationToken)
    {
        var operators = await operatorReader.GetOperatorsAsync(cancellationToken);
        var protocols = operators.Select(o => (ProtocolType)o.ProtocolTypeId).Distinct();

        var allDevices = new List<DeviceVm>();
        await foreach (var devicesCollection in GetDevicesAsync(operators, protocols, cancellationToken))
        {
            allDevices.AddRange(devicesCollection);
        }

        return allDevices;
    }

    private async IAsyncEnumerable<IEnumerable<DeviceVm>> GetDevicesAsync(
        IEnumerable<OperatorVm> operators,
        IEnumerable<ProtocolType> protocols,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var tasks = deviceRegistry.GetReaders(protocols)
            .Select(reader
                => FetchAndProcessDevicesAsync(reader, operators, cancellationToken));
        var fetchTasks = Task.WhenAll(tasks);

        var results = await fetchTasks;
        foreach (var devices in results)
        {
            if (devices.Any())
            {
                yield return devices;
            }
        }
    }

    private async Task<IEnumerable<DeviceVm>> FetchAndProcessDevicesAsync(
        IExternalDeviceReader reader,
        IEnumerable<OperatorVm> operators,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(EncryptionKey, message: "Credential key not found.");
        var @operator = operators.FirstOrDefault(o => (ProtocolType)o.ProtocolTypeId == reader.Protocol);
        if (@operator.Credential is not null)
        {
            await reader.Init(@operator.Credential.Value.Decrypt(EncryptionKey), cancellationToken);
            var devices = await deviceReader.GetDevicesByOperatorAsync(@operator.OperatorId, cancellationToken);
            return await reader.GetDevicesAsync(devices, cancellationToken);
        }
        return [];
    }
}
