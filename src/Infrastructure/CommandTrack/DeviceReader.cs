using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Interfaces;
using TrackHub.Router.Infrastructure.CommandTrack.Mappers;

namespace TrackHub.Router.Infrastructure.CommandTrack;

public sealed class DeviceReader(ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService,
    ICredentialWriter credentialWriter
    ) : CommandTrackReaderBase(httpClientFactory, httpClientService, credentialWriter), IDeviceReader
{
    public async Task<DeviceVm> GetDeviceAsync(DeviceDto deviceDto)
    {
        var url = $"/Device?id={deviceDto.Identifier}";
        var device = await HttpClientService.GetAsync<DevicePosition>(url);
        return device.MapToDeviceVm(deviceDto);
    }

    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceDto> devices)
    {
        var url = $"/Devices{devices.GetIdsQueryString()}";
        var result = await HttpClientService.GetAsync<IEnumerable<DevicePosition>>(url);
        if (result is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result.MapToDeviceVm(devicesDictionary);
    }

    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync()
    {
        var url = "/AllDevices";
        var positions = await HttpClientService.GetAsync<IEnumerable<DevicePosition>>(url);
        return positions is null ? ([]) : positions.MapToDeviceVm();
    }
}
