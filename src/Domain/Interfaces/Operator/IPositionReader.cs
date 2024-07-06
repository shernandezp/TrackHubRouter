namespace TrackHubRouter.Domain.Interfaces.Operator;

public interface IPositionReader
{
    ProtocolType Protocol { get; }
    Task<PositionVm> GetDevicePositionAsync(DeviceVm deviceDto, CancellationToken cancellationToken);
    Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceVm> devices, CancellationToken cancellationToken);
    Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceVm deviceDto, CancellationToken cancellationToken);
    Task Init(CredentialTokenVm credential, CancellationToken cancellationToken = default);
}
