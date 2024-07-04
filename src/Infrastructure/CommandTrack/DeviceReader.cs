using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Interfaces;
using TrackHub.Router.Infrastructure.CommandTrack.Mappers;
using Manager = TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.CommandTrack;

public sealed class DeviceReader(ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService,
    Manager.ICredentialWriter credentialWriter
    ) : CommandTrackReaderBase(httpClientFactory, httpClientService, credentialWriter), IDeviceReader
{
    public async Task<DeviceVm> GetDeviceAsync(DeviceDto deviceDto, CancellationToken cancellationToken)
    {
        var url = $"/Device?id={deviceDto.Identifier}";
        var device = await HttpClientService.GetAsync<DevicePosition>(url, cancellationToken: cancellationToken);
        return device.MapToDeviceVm(deviceDto);
    }

    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceDto> devices, CancellationToken cancellationToken)
    {
        var url = $"/Devices{devices.GetIdsQueryString()}";
        var result = await HttpClientService.GetAsync<IEnumerable<DevicePosition>>(url, cancellationToken: cancellationToken);
        if (result is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return result.MapToDeviceVm(devicesDictionary);
    }

    public async Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
    {
        var url = "/AllDevices";
        var positions = await HttpClientService.GetAsync<IEnumerable<DevicePosition>>(url, cancellationToken: cancellationToken);
        return positions is null ? ([]) : positions.MapToDeviceVm();
    }
}
