namespace TrackHubRouter.Domain.Interfaces.Operator;

public interface IPositionReader
{
    ProtocolType Protocol { get; }
    Task<PositionVm> GetDevicePositionAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken);
    Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken);
    Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceTransporterVm deviceDto, CancellationToken cancellationToken);
    Task Init(CredentialTokenDto credential, CancellationToken cancellationToken = default);
}
