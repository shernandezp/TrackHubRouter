using TrackHubRouter.Domain.Records;

namespace TrackHubRouter.Domain.Interfaces.Operator;

public interface IPositionReader
{
    ProtocolType Protocol { get; }
    Task<PositionVm> GetDevicePositionAsync(DeviceDto deviceDto, CancellationToken cancellationToken);
    Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceDto> devices, CancellationToken cancellationToken);
    Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceDto deviceDto, CancellationToken cancellationToken);
    Task Init(CredentialTokenVm credential, CancellationToken cancellationToken = default);
}
