using TrackHub.Router.Infrastructure.Wialon.Mappers;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.Wialon;

/// <summary>
/// Reader for retrieving position information from Wialon API.
/// </summary>
public sealed class PositionReader(
    ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService)
    : WialonReaderBase(httpClientFactory, httpClientService), IPositionReader
{
    // Wialon search flags
    private const int FlagBasicInfo = 1;       // Basic unit info (id, name)
    private const int FlagPosition = 1024;     // Last position data
    private const int FlagsWithPosition = FlagBasicInfo | FlagPosition; // Combined flags (1025)

    // Message flags for historical data
    private const int MessageFlagPosition = 1;      // Messages with position
    private const int MessageFlagMask = 65281;      // Filter mask for position messages

    /// <summary>
    /// Retrieves the last position of a single device asynchronously.
    /// </summary>
    public async Task<PositionVm> GetDevicePositionAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var parameters = new
        {
            id = deviceDto.Identifier,
            flags = FlagsWithPosition
        };

        var result = await PostAsync<SingleItemResponse>("core/search_item", parameters, cancellationToken);
        return result?.Item is null
            ? throw new InvalidOperationException($"Device not found: {deviceDto.Identifier}")
            : result.Item.Value.MapToPositionVm(deviceDto);
    }

    /// <summary>
    /// Retrieves the last positions of multiple devices asynchronously.
    /// </summary>
    public async Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
    {
        var parameters = new
        {
            spec = new
            {
                itemsType = "avl_unit",
                propName = "sys_name",
                propValueMask = "*",
                sortType = "sys_name"
            },
            force = 1,
            flags = FlagsWithPosition,
            from = 0,
            to = 0
        };

        var result = await PostAsync<SearchResponse>("core/search_items", parameters, cancellationToken);
        if (result?.Items is null || !result.Items.Any())
        {
            return [];
        }

        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result.Items.MapToPositionVm(devicesDictionary).Distinct();
    }

    /// <summary>
    /// Retrieves the positions of a device within a specified time range asynchronously.
    /// Uses messages/load_interval to get historical position data.
    /// </summary>
    public async Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var timeFrom = from.ToUnixTimeSeconds();
        var timeTo = to.ToUnixTimeSeconds();

        var parameters = new
        {
            itemId = deviceDto.Identifier,
            timeFrom,
            timeTo,
            flags = MessageFlagPosition,
            flagsMask = MessageFlagMask,
            loadCount = 0xFFFFFFFF  // Load all messages in range
        };

        var result = await PostAsync<MessageResponse>("messages/load_interval", parameters, cancellationToken);
        return result?.Messages is null ? [] : result.Messages.MapToPositionVm(deviceDto);
    }
}
