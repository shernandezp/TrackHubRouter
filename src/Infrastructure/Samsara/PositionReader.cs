using Common.Domain.Extensions;
using TrackHub.Router.Infrastructure.Samsara.Mappers;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Samsara;

/// <summary>
/// Reader for retrieving position information from Samsara API.
/// </summary>
public sealed class PositionReader(
    ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService)
    : SamsaraReaderBase(httpClientFactory, httpClientService)
{
    /// <summary>
    /// Retrieves the last position of a single device asynchronously.
    /// </summary>
    public async Task<PositionVm> GetDevicePositionAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"fleet/vehicles/stats?vehicleIds={deviceDto.Serial}&types=gps";
        var result = await HttpClientService.GetAsync<VehicleStatsResponse>(url, cancellationToken: cancellationToken);
        
        var vehicle = result?.Data?.FirstOrDefault();
        return vehicle is null
            ? throw new InvalidOperationException($"Device not found: {deviceDto.Serial}")
            : vehicle.Value.MapToPositionVm(deviceDto);
    }

    /// <summary>
    /// Retrieves the last positions of multiple devices asynchronously.
    /// </summary>
    public async Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
    {
        var vehicleIds = string.Join(",", devices.Select(d => d.Serial));
        var url = $"fleet/vehicles/stats?vehicleIds={vehicleIds}&types=gps";
        
        var result = await HttpClientService.GetAsync<VehicleStatsResponse>(url, cancellationToken: cancellationToken);
        if (result?.Data is null || !result.Data.Any())
        {
            return [];
        }

        var devicesDictionary = devices.ToDictionary(device => device.Serial, device => device);
        return result.Data.MapToPositionVm(devicesDictionary).Distinct();
    }

    /// <summary>
    /// Retrieves the positions of a device within a specified time range asynchronously.
    /// Uses /fleet/vehicles/stats/history endpoint.
    /// </summary>
    public async Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var startTime = from.ToIso8601String();
        var endTime = to.ToIso8601String();
        var url = $"fleet/vehicles/stats/history?vehicleIds={deviceDto.Serial}&types=gps&startTime={startTime}&endTime={endTime}";
        
        var result = await HttpClientService.GetAsync<VehicleHistoryResponse>(url, cancellationToken: cancellationToken);
        return result?.Data is null ? [] : result.Data.SelectMany(v => v.MapToPositionVm(deviceDto));
    }
}
