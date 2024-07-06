using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.Traccar.Adapters;

//Adapter is required here to make the Init method asynchronous
public sealed class PositionReaderAdapter(PositionReader positionReader) : IPositionReader
{
    public ProtocolType Protocol => positionReader.Protocol;

    public Task Init(CredentialTokenVm credential, CancellationToken cancellationToken = default)
        => Task.Run(() => positionReader.Init(credential), cancellationToken);

    public Task<PositionVm> GetDevicePositionAsync(DeviceVm deviceDto, CancellationToken cancellationToken)
        => positionReader.GetDevicePositionAsync(deviceDto, cancellationToken);

    public Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceVm> devices, CancellationToken cancellationToken)
        => positionReader.GetDevicePositionAsync(devices, cancellationToken);

    public Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceVm deviceDto, CancellationToken cancellationToken)
        => positionReader.GetPositionAsync(from, to, deviceDto, cancellationToken);

}
