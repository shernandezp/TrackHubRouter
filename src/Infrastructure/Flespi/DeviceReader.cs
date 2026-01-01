using TrackHub.Router.Infrastructure.Flespi.Mappers;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Flespi;

/// <summary>
/// Reader for retrieving device information from Flespi API.
/// </summary>
public sealed class DeviceReader(
    ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService)
    : FlespiReaderBase(httpClientFactory, httpClientService)
{
    /// <summary>
    /// Retrieves a single device asynchronously based on the provided device DTO.
    /// </summary>
    public async Task<DeviceVm> GetDeviceAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"gw/devices/{deviceDto.Identifier}";
        var result = await HttpClientService.GetAsync<DeviceListResponse>(url, cancellationToken: cancellationToken);
        
        var device = result?.Result?.FirstOrDefault();
        return device is null
            ? throw new InvalidOperationException($"Device not found: {deviceDto.Identifier}")
            : device.Value.MapToDeviceVm(deviceDto);
    }

    /// <summary>
    /// Retrieves multiple devices asynchronously based on the provided device DTOs.
    /// </summary>
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
    {
        var url = "gw/devices/all";
        var result = await HttpClientService.GetAsync<DeviceListResponse>(url, cancellationToken: cancellationToken);
        
        if (result?.Result is null || result.Result.Count == 0)
        {
            return [];
        }

        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result.Result
            .Where(d => devicesDictionary.ContainsKey((int)d.Id))
            .MapToDeviceVm(devicesDictionary);
    }

    /// <summary>
    /// Retrieves all devices asynchronously.
    /// </summary>
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
    {
        var url = "gw/devices/all";
        var result = await HttpClientService.GetAsync<DeviceListResponse>(url, cancellationToken: cancellationToken);

        return result?.Result is null ? [] : result.Result.MapToDeviceVm().Distinct();
    }
}
