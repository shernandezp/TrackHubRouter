using TrackHub.Router.Infrastructure.GpsGate.Mappers;
using TrackHub.Router.Infrastructure.GpsGate.Models;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Models;

namespace TrackHub.Router.Infrastructure.GpsGate;

// This class represents a reader for GpsGate api - devices.
public sealed class DeviceReader(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService)
    : GpsGateReaderBase(httpClientFactory, httpClientService)
{

    /// <summary>
    /// Retrieves a single device asynchronously based on the provided device DTO. 
    /// </summary>
    /// <param name="deviceDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the device as an DeviceVm.</returns>
    public async Task<DeviceVm> GetDeviceAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"api/v.1/{ApplicationId}/users/{UserId}/devices/{deviceDto.Identifier}";
        var device = await HttpClientService.GetAsync<Device>(url, cancellationToken: cancellationToken);
        return device.MapToDeviceVm(deviceDto);
    }

    /// <summary>
    /// Retrieves multiple devices asynchronously based on the provided device DTOs.
    /// </summary>
    /// <param name="devices"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the devices as a collection of DeviceVm.</returns>
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
    {
        var url = $"api/v.1/{ApplicationId}/users/{UserId}/devices";
        var result = await HttpClientService.GetAsync<IEnumerable<Device>>(url, cancellationToken: cancellationToken);
        if (result is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result
            .Where(d => devicesDictionary.ContainsKey(d.Id))
            .MapToDeviceVm(devicesDictionary);
    }

    /// <summary>
    /// Retrieves all devices asynchronously.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns all devices as a collection of DeviceVm.</returns>
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
    {
        var url = $"api/v.1/{ApplicationId}/users/{UserId}/devices";
        var positions = await HttpClientService.GetAsync<IEnumerable<Device>>(url, cancellationToken: cancellationToken);
        return positions is null ? ([]) : positions.MapToDeviceVm().Distinct();
    }
}
