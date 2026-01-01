using TrackHub.Router.Infrastructure.Navixy.Mappers;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.Navixy;

/// <summary>
/// Reader for retrieving device (tracker) information from Navixy API.
/// </summary>
public sealed class DeviceReader(
    ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService)
    : NavixyReaderBase(httpClientFactory, httpClientService), IExternalDeviceReader
{
    /// <summary>
    /// Retrieves a single device asynchronously based on the provided device DTO.
    /// </summary>
    public async Task<DeviceVm> GetDeviceAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var result = await HttpClientService.PostAsync<TrackerListResponse>(
            $"{BaseUrl}/v2/tracker/list", new { hash = Hash }, cancellationToken);
        
        var tracker = result?.List?.FirstOrDefault(t => t.Tracker_id == deviceDto.Identifier);
        return tracker is null
            ? throw new InvalidOperationException($"Device not found: {deviceDto.Identifier}")
            : tracker.Value.MapToDeviceVm(deviceDto);
    }

    /// <summary>
    /// Retrieves multiple devices asynchronously based on the provided device DTOs.
    /// </summary>
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
    {
        var result = await HttpClientService.PostAsync<TrackerListResponse>(
            $"{BaseUrl}/v2/tracker/list", new { hash = Hash }, cancellationToken);
        
        if (result?.List is null || !result.List.Any())
        {
            return [];
        }

        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result.List
            .Where(t => devicesDictionary.ContainsKey((int)t.Tracker_id))
            .MapToDeviceVm(devicesDictionary);
    }

    /// <summary>
    /// Retrieves all devices asynchronously.
    /// </summary>
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
    {
        var result = await HttpClientService.PostAsync<TrackerListResponse>(
            $"{BaseUrl}/v2/tracker/list", new { hash = Hash }, cancellationToken);

        return result?.List is null ? [] : result.List.MapToDeviceVm().Distinct();
    }
}
