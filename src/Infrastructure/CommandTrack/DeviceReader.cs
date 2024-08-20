using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Interfaces;
using TrackHub.Router.Infrastructure.CommandTrack.Mappers;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.CommandTrack;

// This class represents a device reader that retrieves device information from CommandTrack API
public sealed class DeviceReader(ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService,
    ICredentialWriter credentialWriter
    ) : CommandTrackReaderBase(httpClientFactory, httpClientService, credentialWriter), IExternalDeviceReader
{
    public async Task<DeviceVm> GetDeviceAsync(DeviceOperatorVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"DataConnectAPI/api/Device?id={deviceDto.Identifier}";
        var device = await HttpClientService.GetAsync<DevicePosition>(url, Header, cancellationToken);
        return device.MapToDeviceVm(deviceDto);
    }

    // Retrieves a single device asynchronously
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceOperatorVm> devices, CancellationToken cancellationToken)
    {
        var url = $"DataConnectAPI/api/Devices{devices.GetIdsQueryString()}";
        var result = await HttpClientService.GetAsync<IEnumerable<DevicePosition>>(url, Header, cancellationToken);
        if (result is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result.MapToDeviceVm(devicesDictionary);
    }

    // Retrieves multiple devices asynchronously
    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
    {
        var url = "DataConnectAPI/api/AllDevices";
        var positions = await HttpClientService.GetAsync<IEnumerable<DevicePosition>>(url, Header, cancellationToken);
        return positions is null ? ([]) : positions.MapToDeviceVm();
    }
}
