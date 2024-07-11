namespace TrackHubRouter.Domain.Interfaces.Operator;

public interface IExternalDeviceReader
{
    ProtocolType Protocol { get; }
    Task Init(CredentialTokenDto credential, CancellationToken cancellationToken = default);
    Task<ExternalDeviceVm> GetDeviceAsync(DeviceVm deviceDto, CancellationToken cancellationToken);
    Task<IEnumerable<ExternalDeviceVm>> GetDevicesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<ExternalDeviceVm>> GetDevicesAsync(IEnumerable<DeviceVm> devices, CancellationToken cancellationToken);
}
