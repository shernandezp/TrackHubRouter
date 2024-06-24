namespace TrackHub.Router.Infrastructure.CommandTrack.Interfaces;
public interface IPositionReader
{
    Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceDto deviceDto);
    Task<PositionVm> GetPositionAsync(DeviceDto deviceDto);
    Task<IEnumerable<PositionVm>> GetPositionAsync(IEnumerable<DeviceDto> devices);
    Task Init(Guid credential, CancellationToken cancellationToken);
}
