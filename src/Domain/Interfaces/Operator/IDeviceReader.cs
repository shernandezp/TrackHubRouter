using TrackHubRouter.Domain.Records;

namespace TrackHubRouter.Domain.Interfaces.Operator;

public interface IDeviceReader
{
    Task Init(CredentialTokenVm credential, CancellationToken cancellationToken = default);
    Task<DeviceVm> GetDeviceAsync(DeviceDto deviceDto, CancellationToken cancellationToken);
    Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceDto> devices, CancellationToken cancellationToken);
}
