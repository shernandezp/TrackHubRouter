using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Interfaces;
using TrackHub.Router.Infrastructure.CommandTrack.Mappers;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.CommandTrack;

public sealed class DeviceReader(ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService,
    ICredentialWriter credentialWriter
    ) : CommandTrackReaderBase(httpClientFactory, httpClientService, credentialWriter), IExternalDeviceReader
{
    public async Task<ExternalDeviceVm> GetDeviceAsync(DeviceVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"DataConnectAPI/api/Device?id={deviceDto.Identifier}";
        var device = await HttpClientService.GetAsync<DevicePosition>(url, Header, cancellationToken);
        return device.MapToDeviceVm(deviceDto);
    }

    public async Task<IEnumerable<ExternalDeviceVm>> GetDevicesAsync(IEnumerable<DeviceVm> devices, CancellationToken cancellationToken)
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

    public async Task<IEnumerable<ExternalDeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
    {
        var url = "DataConnectAPI/api/AllDevices";
        var positions = await HttpClientService.GetAsync<IEnumerable<DevicePosition>>(url, Header, cancellationToken);
        return positions is null ? ([]) : positions.MapToDeviceVm();
    }
}
