using Common.Application.Attributes;
using Common.Domain.Constants;
using System.Runtime.CompilerServices;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Interfaces.Manager;
using Common.Application.Interfaces;

namespace TrackHubRouter.Application.Devices.Queries.Get;

[Authorize(Resource = Resources.Devices, Action = Actions.Read)]
public readonly record struct GetDevicesQuery() : IRequest<IEnumerable<ExternalDeviceVm>>;

public class GetDevicesQueryHandler(IOperatorReader operatorReader,
    IDeviceRegistry deviceRegistry,
    IDeviceReader deviceReader,
    IUser user)
    : IRequestHandler<GetDevicesQuery, IEnumerable<ExternalDeviceVm>>
{

    private Guid UserId { get; } = user.Id is null ? throw new UnauthorizedAccessException() : new Guid(user.Id);

    public async Task<IEnumerable<ExternalDeviceVm>> Handle(GetDevicesQuery request, CancellationToken cancellationToken)
    {
        var operators = await operatorReader.GetOperatorsAsync(UserId, cancellationToken);
        var protocols = operators.Select(o => o.ProtocolType).Distinct();

        var allDevices = new List<ExternalDeviceVm>();
        await foreach (var devicesCollection in GetDevicesAsync(operators, protocols, cancellationToken))
        {
            allDevices.AddRange(devicesCollection);
        }

        return allDevices;
    }

    private async IAsyncEnumerable<IEnumerable<ExternalDeviceVm>> GetDevicesAsync(
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

    private async Task<IEnumerable<ExternalDeviceVm>> FetchAndProcessDevicesAsync(
        IExternalDeviceReader reader,
        IEnumerable<OperatorVm> operators,
        CancellationToken cancellationToken)
    {
        var @operator = operators.FirstOrDefault(o => o.ProtocolType == reader.Protocol);
        if (@operator.Credential is not null)
        {
            await reader.Init(@operator.Credential.Value, cancellationToken);
            var devices = await deviceReader.GetDevicesByOperatorAsync(UserId, @operator.OperatorId, cancellationToken);
            return await reader.GetDevicesAsync(devices, cancellationToken);
        }
        return [];
    }

}
