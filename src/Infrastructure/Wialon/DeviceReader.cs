using TrackHub.Router.Infrastructure.Wialon.Mappers;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.Wialon;

/// <summary>
/// Reader for retrieving device information from Wialon API.
/// </summary>
public sealed class DeviceReader(
    ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService)
    : WialonReaderBase(httpClientFactory, httpClientService), IExternalDeviceReader
{
    // Wialon search flags
    private const int FlagBasicInfo = 1;       // Basic unit info (id, name)

    /// <summary>
    /// Retrieves a single device asynchronously based on the provided device DTO.
    /// </summary>
    public async Task<DeviceVm> GetDeviceAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var parameters = new
        {
            id = deviceDto.Identifier,
            flags = FlagBasicInfo
        };

        var result = await PostAsync<SingleItemResponse>("core/search_item", parameters, cancellationToken);
        return result?.Item is null
            ? throw new InvalidOperationException($"Device not found: {deviceDto.Identifier}")
            : result.Item.Value.MapToDeviceVm(deviceDto);
    }

    /// <summary>
    /// Retrieves multiple devices asynchronously based on the provided device DTOs.
    /// </summary>
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
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
            flags = FlagBasicInfo,
            from = 0,
            to = 0
        };

        var result = await PostAsync<SearchResponse>("core/search_items", parameters, cancellationToken);
        if (result?.Items is null || !result.Items.Any())
        {
            return [];
        }

        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result.Items.MapToDeviceVm(devicesDictionary);
    }

    /// <summary>
    /// Retrieves all devices asynchronously.
    /// </summary>
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
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
            flags = FlagBasicInfo,
            from = 0,
            to = 0
        };

        var result = await PostAsync<SearchResponse>("core/search_items", parameters, cancellationToken);
        return result?.Items is null ? [] : result.Items.MapToDeviceVm().Distinct();
    }
}
