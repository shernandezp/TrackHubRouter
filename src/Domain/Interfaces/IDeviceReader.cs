using TrackHubRouter.Domain.Records;

namespace TrackHubRouter.Domain.Interfaces;

public interface IDeviceReader
{
    Task Init(CredentialVm credential, CredentialTokenVm? credentialToken, CancellationToken cancellationToken = default);
    Task<DeviceVm> GetDeviceAsync(DeviceDto deviceDto);
    Task<IEnumerable<DeviceVm>> GetDevicesAsync();
    Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceDto> devices);
}
