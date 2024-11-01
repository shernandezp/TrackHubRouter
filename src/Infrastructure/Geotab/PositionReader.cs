using Geotab.Checkmate.ObjectModel;
using TrackHub.Router.Infrastructure.Geotab.Mappers;
using TrackHubRouter.Domain.Interfaces.Operator;
using TrackHubRouter.Domain.Models;

namespace TrackHub.Router.Infrastructure.Geotab;

public sealed class PositionReader() : GeotabReaderBase(), IPositionReader
{
    public async Task<PositionVm> GetDevicePositionAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var search = new DeviceStatusInfoSearch
        {
            DeviceSearch = new DeviceSearch
            {
                Id = Id.Create(deviceDto.Identifier)
            }
        };
        var position = await GeotabApi!.CallAsync<DeviceStatusInfo>("Get", typeof(DeviceStatusInfo), search, cancellationToken);
        return position!.MapToPositionVm(deviceDto);
    }

    public async Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
    {
        var search = new DeviceStatusInfoSearch
        {
            DeviceSearch = new DeviceSearch
            {
                DeviceIds = devices.Select(device => Id.Create(device.Identifier))
            }
        };
        var positions = await GeotabApi!.CallAsync<IEnumerable<DeviceStatusInfo>>("Get", typeof(DeviceStatusInfo), search, cancellationToken);
        if (positions is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Name, device => device);
        return positions.MapToPositionVm(devicesDictionary).Distinct();
    }

    public async Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var logRecordSearch = new LogRecordSearch
        {
            DeviceSearch = new DeviceSearch(Id.Create(deviceDto.Identifier)),
            FromDate = from.DateTime,
            ToDate = to.DateTime
        };
        var positions = await GeotabApi!.CallAsync<IEnumerable<LogRecord>>("Get", typeof(LogRecord), new { search = logRecordSearch }, cancellationToken);
        return positions is null ? ([]) : positions.MapToPositionVm(deviceDto);
    }
}
