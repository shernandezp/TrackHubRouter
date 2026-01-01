using TrackHub.Router.Infrastructure.Flespi.Mappers;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Flespi;

/// <summary>
/// Reader for retrieving position information from Flespi API.
/// </summary>
public sealed class PositionReader(
    ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService)
    : FlespiReaderBase(httpClientFactory, httpClientService)
{
    /// <summary>
    /// Retrieves the last position of a single device asynchronously.
    /// Uses /gw/devices/{id}/messages with limit=1 and reverse=true to get latest message.
    /// </summary>
    public async Task<PositionVm> GetDevicePositionAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"gw/devices/{deviceDto.Identifier}/messages?data=%7B%22reverse%22%3Atrue%7D";
        var result = await HttpClientService.GetAsync<MessageListResponse>(url, cancellationToken: cancellationToken);
        
        var message = result?.Result?.FirstOrDefault();
        return message is null
            ? throw new InvalidOperationException($"No position data found for device: {deviceDto.Identifier}")
            : message.Value.MapToPositionVm(deviceDto);
    }

    /// <summary>
    /// Retrieves the last positions of multiple devices asynchronously.
    /// </summary>
    public async Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
    {
        var positions = new List<PositionVm>();
        
        foreach (var device in devices)
        {
            try
            {
                var position = await GetDevicePositionAsync(device, cancellationToken);
                positions.Add(position);
            }
            catch
            {
                // Skip devices without position data
            }
        }
        
        return positions.Distinct();
    }

    /// <summary>
    /// Retrieves the positions of a device within a specified time range asynchronously.
    /// Uses /gw/devices/{id}/messages with time filters (Unix seconds).
    /// </summary>
    public async Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var fromSeconds = from.ToUnixTimeSeconds();
        var toSeconds = to.ToUnixTimeSeconds();
        
        // Flespi uses generalized-time filter format
        var url = $"gw/devices/{deviceDto.Identifier}/messages?data=%7B%22from%22%3A{fromSeconds}%2C%22to%22%3A{toSeconds}%7D";
        var result = await HttpClientService.GetAsync<MessageListResponse>(url, cancellationToken: cancellationToken);

        return result?.Result is null ? [] : result.Result.MapToPositionVm(deviceDto);
    }
}
