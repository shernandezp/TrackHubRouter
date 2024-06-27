using TrackHubRouter.Domain.Records;

namespace TrackHubRouter.Domain.Interfaces;

public interface IPositionReader
{
    Task<PositionVm> GetDevicePositionAsync(DeviceDto deviceDto);
    Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceDto> devices);
    Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceDto deviceDto);
    Task Init(CredentialVm credential, CancellationToken cancellationToken);
}
