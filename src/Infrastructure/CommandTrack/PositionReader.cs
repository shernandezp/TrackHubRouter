namespace TrackHub.Router.Infrastructure.CommandTrack;

using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Records;
using TrackHub.Router.Infrastructure.CommandTrack.Mappers;
using Common.Domain.Extensions;
using TrackHubRouter.Domain.Extensions;

public sealed class PositionReader(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService) : IPositionReader
{
    private HttpClient? _httpClient;

    public async Task Init(CredentialVm credential, CancellationToken cancellationToken)
    {
        _httpClient = await httpClientFactory.CreateClientAsync(credential.CredentialId, cancellationToken);
        httpClientService.Init(_httpClient, $"{ProtocolType.CommandTrack}");
    }

    public async Task<PositionVm> GetDevicePositionAsync(DeviceDto deviceDto)
    {
        var url = $"api/Device/{deviceDto.Name}";
        var position = await httpClientService.GetAsync<DevicePosition>(url);
        return position.MapToPositionVm(deviceDto);
    }

    public async Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceDto> devices)
    {
        var url = $"api/Devices{devices.GetIdsQueryString()}";
        var positions = await httpClientService.GetAsync<IEnumerable<DevicePosition>>(url);
        if (positions is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Name, device => device);
        return positions.MapToPositionVm(devicesDictionary);
    }

    public async Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceDto deviceDto)
    {
        var url = $"api/Position/{deviceDto.Name}/{from.ToIso8601String()}/{to.ToIso8601String()}";
        var positions = await httpClientService.GetAsync<IEnumerable<Position>>(url);
        return positions is null ? ([]) : positions.MapToPositionVm(deviceDto);
    }
}
