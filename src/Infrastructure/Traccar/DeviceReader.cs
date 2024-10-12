using TrackHub.Router.Infrastructure.Traccar.Mappers;
using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Traccar;


// This class represents a reader for Traccar api - devices.
public sealed class DeviceReader(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService)
    : TraccarReaderBase(httpClientFactory, httpClientService)
{

    // Retrieves a single device asynchronously based on the provided device DTO.
    // Returns the device as an ExternalDeviceVm.
    public async Task<DeviceVm> GetDeviceAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"api/devices?id={deviceDto.Identifier}";
        var device = await HttpClientService.GetAsync<Device>(url, cancellationToken: cancellationToken);
        return device.MapToDeviceVm(deviceDto);
    }

    // Retrieves multiple devices asynchronously based on the provided device DTOs.
    // Returns the devices as a collection of ExternalDeviceVm.
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
    {
        var url = $"api/devices?{devices.GetIdsQueryString()}";
        var result = await HttpClientService.GetAsync<IEnumerable<Device>>(url, cancellationToken: cancellationToken);
        if (result is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result.MapToDeviceVm(devicesDictionary);
    }

    // Retrieves all devices asynchronously.
    // Returns all devices as a collection of ExternalDeviceVm.
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
    {
        var url = "api/devices?all=true";
        var positions = await HttpClientService.GetAsync<IEnumerable<Device>>(url, cancellationToken: cancellationToken);
        return positions is null ? ([]) : positions.MapToDeviceVm();
    }
}
