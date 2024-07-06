using Common.Application.Attributes;
using Common.Domain.Constants;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.Devices.Queries.GetByOperator;

[Authorize(Resource = Resources.Devices, Action = Actions.Read)]
public readonly record struct GetDevicesByOperatorQuery(Guid OperatorId) : IRequest<IEnumerable<ExternalDeviceVm>>;

public class GetDevicesByOperatorQueryHandler(IOperatorReader operatorReader,
    IDeviceRegistry deviceRegistry)
    : IRequestHandler<GetDevicesByOperatorQuery, IEnumerable<ExternalDeviceVm>>
{

    public async Task<IEnumerable<ExternalDeviceVm>> Handle(GetDevicesByOperatorQuery request, CancellationToken cancellationToken)
    {
        var @operator = await operatorReader.GetOperatorAsync(request.OperatorId, cancellationToken);
        return await GetDevicesAsync(@operator, cancellationToken);
    }

    private async Task<IEnumerable<ExternalDeviceVm>> GetDevicesAsync(
        OperatorVm @operator,
        CancellationToken cancellationToken)
    {
        var reader = deviceRegistry.GetReader(@operator.ProtocolType);
        return await FetchAndProcessDevicesAsync(reader, @operator, cancellationToken);

    }

    private async Task<IEnumerable<ExternalDeviceVm>> FetchAndProcessDevicesAsync(
        IExternalDeviceReader reader,
        OperatorVm @operator,
        CancellationToken cancellationToken)
    {
        if (@operator.Credential is not null)
        {
            await reader.Init(@operator.Credential.Value, cancellationToken);
            return await reader.GetDevicesAsync(cancellationToken);
        }
        return [];
    }

}
