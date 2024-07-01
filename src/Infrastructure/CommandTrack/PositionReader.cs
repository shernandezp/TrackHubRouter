namespace TrackHub.Router.Infrastructure.CommandTrack;

using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Records;
using TrackHub.Router.Infrastructure.CommandTrack.Mappers;
using Common.Domain.Extensions;
using TrackHubRouter.Domain.Extensions;

public sealed class PositionReader(ICredentialHttpClientFactory httpClientFactory, 
    IHttpClientService httpClientService,
    ICredentialWriter credentialWriter
    ) : CommandTrackReaderBase(httpClientFactory, httpClientService, credentialWriter), IPositionReader
{
    public async Task<PositionVm> GetDevicePositionAsync(DeviceDto deviceDto)
    {
        var url = $"/DataConnect/api/Device/{deviceDto.Name}";
        var position = await HttpClientService.GetAsync<DevicePosition>(url, Header);
        return position.MapToPositionVm(deviceDto);
    }

    public async Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceDto> devices)
    {
        var url = $"/DataConnect/api/Devices{devices.GetIdsQueryString()}";
        var positions = await HttpClientService.GetAsync<IEnumerable<DevicePosition>>(url, Header);
        if (positions is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Name, device => device);
        return positions.MapToPositionVm(devicesDictionary);
    }

    public async Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceDto deviceDto)
    {
        var url = $"/DataConnect/api/Position/{deviceDto.Name}/{from.ToIso8601String()}/{to.ToIso8601String()}";
        var positions = await HttpClientService.GetAsync<IEnumerable<Position>>(url, Header);
        return positions is null ? ([]) : positions.MapToPositionVm(deviceDto);
    }
}
