using TrackHub.Router.Infrastructure.Traccar.Mappers;
using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Traccar;

public sealed class DeviceReader(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService)
    : TraccarReaderBase(httpClientFactory, httpClientService)
{

    public async Task<ExternalDeviceVm> GetDeviceAsync(DeviceVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"api/devices?id={deviceDto.Identifier}";
        var device = await HttpClientService.GetAsync<Device>(url, cancellationToken: cancellationToken);
        return device.MapToDeviceVm(deviceDto);
    }

    public async Task<IEnumerable<ExternalDeviceVm>> GetDevicesAsync(IEnumerable<DeviceVm> devices, CancellationToken cancellationToken)
    {
        var url = $"api/devices{devices.GetIdsQueryString()}";
        var result = await HttpClientService.GetAsync<IEnumerable<Device>>(url, cancellationToken: cancellationToken);
        if (result is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result.MapToDeviceVm(devicesDictionary);
    }

    public async Task<IEnumerable<ExternalDeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
    {
        var url = "api/devices?all=true";
        var positions = await HttpClientService.GetAsync<IEnumerable<Device>>(url, cancellationToken: cancellationToken);
        return positions is null ? ([]) : positions.MapToDeviceVm();
    }
}
