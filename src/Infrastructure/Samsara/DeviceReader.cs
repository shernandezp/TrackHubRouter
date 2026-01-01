using TrackHub.Router.Infrastructure.Samsara.Mappers;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Samsara;

/// <summary>
/// Reader for retrieving device (vehicle) information from Samsara API.
/// </summary>
public sealed class DeviceReader(
    ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService)
    : SamsaraReaderBase(httpClientFactory, httpClientService)
{
    /// <summary>
    /// Retrieves a single device asynchronously based on the provided device DTO.
    /// Uses the stats endpoint filtered by vehicle ID.
    /// </summary>
    public async Task<DeviceVm> GetDeviceAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"fleet/vehicles/stats?vehicleIds={deviceDto.Serial}";
        var result = await HttpClientService.GetAsync<VehicleStatsResponse>(url, cancellationToken: cancellationToken);
        
        var vehicle = result?.Data?.FirstOrDefault();
        return vehicle is null
            ? throw new InvalidOperationException($"Device not found: {deviceDto.Serial}")
            : vehicle.Value.MapToDeviceVm(deviceDto);
    }

    /// <summary>
    /// Retrieves multiple devices asynchronously based on the provided device DTOs.
    /// </summary>
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
    {
        var vehicleIds = string.Join(",", devices.Select(d => d.Serial));
        var url = $"fleet/vehicles/stats?vehicleIds={vehicleIds}";
        
        var result = await HttpClientService.GetAsync<VehicleStatsResponse>(url, cancellationToken: cancellationToken);
        if (result?.Data is null || !result.Data.Any())
        {
            return [];
        }

        var devicesDictionary = devices.ToDictionary(device => device.Serial, device => device);
        return result.Data.MapToDeviceVm(devicesDictionary);
    }

    /// <summary>
    /// Retrieves all devices asynchronously.
    /// </summary>
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
    {
        var url = "fleet/vehicles/stats";
        var result = await HttpClientService.GetAsync<VehicleStatsResponse>(url, cancellationToken: cancellationToken);

        return result?.Data is null ? [] : result.Data.MapToDeviceVm().Distinct();
    }
}
