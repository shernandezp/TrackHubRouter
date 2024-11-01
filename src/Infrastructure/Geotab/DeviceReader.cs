using Geotab.Checkmate.ObjectModel;
using TrackHub.Router.Infrastructure.Geotab.Mappers;
using TrackHubRouter.Domain.Interfaces.Operator;
using TrackHubRouter.Domain.Models;

namespace TrackHub.Router.Infrastructure.Geotab;

// This class represents a device reader that retrieves device information from CommandTrack API
public sealed class DeviceReader() : GeotabReaderBase(), IExternalDeviceReader
{
    public async Task<DeviceVm> GetDeviceAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var deviceSearch = new DeviceSearch(Id.Create(deviceDto.Identifier));
        var device = await GeotabApi!.CallAsync<Device>("Get", typeof(Device), new { search = deviceSearch }, cancellationToken);
        return device!.MapToDeviceVm(deviceDto);
    }

    // Retrieves a single device asynchronously
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
    {
        var deviceSearch = new DeviceSearch
        {
            DeviceIds = devices.Select(device => Id.Create(device.Identifier))
        };
        var result = await GeotabApi!.CallAsync<IEnumerable<Device>>("Get", typeof(Device), new { search = deviceSearch }, cancellationToken);
        if (result is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result.MapToDeviceVm(devicesDictionary);
    }

    // Retrieves multiple devices asynchronously
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
    {
        var devices = await GeotabApi!.CallAsync<IEnumerable<Device>>("Get", typeof(Device), cancellationToken);
        return devices is null ? ([]) : devices.MapToDeviceVm().Distinct();
    }
}
