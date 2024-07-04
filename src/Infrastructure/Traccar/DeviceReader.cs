using TrackHub.Router.Infrastructure.Traccar.Mappers;
using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.Traccar;

public sealed class DeviceReader(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService)
    : TraccarReaderBase(httpClientFactory, httpClientService), IDeviceReader
{

    public async Task<DeviceVm> GetDeviceAsync(DeviceDto deviceDto, CancellationToken cancellationToken)
    {
        var url = $"/devices?id={deviceDto.Identifier}";
        var device = await HttpClientService.GetAsync<Device>(url, cancellationToken: cancellationToken);
        return device.MapToDeviceVm(deviceDto);
    }

    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceDto> devices, CancellationToken cancellationToken)
    {
        var url = $"/devices{devices.GetIdsQueryString()}";
        var result = await HttpClientService.GetAsync<IEnumerable<Device>>(url, cancellationToken: cancellationToken);
        if (result is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result.MapToDeviceVm(devicesDictionary);
    }

    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
    {
        var url = "/positions?all=true";
        var positions = await HttpClientService.GetAsync<IEnumerable<Device>>(url, cancellationToken: cancellationToken);
        return positions is null ? ([]) : positions.MapToDeviceVm();
    }
}
