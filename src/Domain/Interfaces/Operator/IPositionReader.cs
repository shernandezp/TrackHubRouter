namespace TrackHubRouter.Domain.Interfaces.Operator;

public interface IPositionReader
{
    ProtocolType Protocol { get; }
    Task<PositionVm> GetDevicePositionAsync(DeviceOperatorVm deviceDto, CancellationToken cancellationToken);
    Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceOperatorVm> devices, CancellationToken cancellationToken);
    Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceOperatorVm deviceDto, CancellationToken cancellationToken);
    Task Init(CredentialTokenDto credential, CancellationToken cancellationToken = default);
}
