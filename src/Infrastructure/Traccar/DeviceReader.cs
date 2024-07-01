using TrackHub.Router.Infrastructure.Traccar.Mappers;
using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Traccar;

public sealed class DeviceReader(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService)
    : TraccarReaderBase(httpClientFactory, httpClientService), IDeviceReader
{

    public async Task<DeviceVm> GetDeviceAsync(DeviceDto deviceDto)
    {
        var url = $"/devices?id={deviceDto.Identifier}";
        var device = await HttpClientService.GetAsync<Device>(url);
        return device.MapToDeviceVm(deviceDto);
    }

    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceDto> devices)
    {
        var url = $"/devices{devices.GetIdsQueryString()}";
        var result = await HttpClientService.GetAsync<IEnumerable<Device>>(url);
        if (result is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result.MapToDeviceVm(devicesDictionary);
    }

    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync()
    {
        var url = "/positions?all=true";
        var positions = await HttpClientService.GetAsync<IEnumerable<Device>>(url);
        return positions is null ? ([]) : positions.MapToDeviceVm();
    }
}
