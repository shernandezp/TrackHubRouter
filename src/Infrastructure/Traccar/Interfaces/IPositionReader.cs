namespace TrackHub.Router.Infrastructure.Traccar.Interfaces;

public interface IPositionReader
{
    Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceDto deviceDto);
    Task<PositionVm> GetPositionAsync(DeviceDto deviceDto);
    Task<IEnumerable<PositionVm>> GetPositionAsync(IEnumerable<DeviceDto> devices);
    Task Init(CredentialVm credential, CancellationToken cancellationToken);
}
