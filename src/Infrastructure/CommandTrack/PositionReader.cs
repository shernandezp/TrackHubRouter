namespace TrackHub.Router.Infrastructure.CommandTrack;

using TrackHub.Router.Infrastructure.CommandTrack.Entities;
using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Records;
using TrackHub.Router.Infrastructure.CommandTrack.Mappers;
using TrackHub.Router.Infrastructure.CommandTrack.Interfaces;

public sealed class PositionReader(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService) : IPositionReader
{
    private HttpClient? _httpClient;

    public async Task Init(Guid credential, CancellationToken cancellationToken)
    {
        _httpClient = await httpClientFactory.CreateClientAsync(credential, cancellationToken);
        httpClientService.Init(_httpClient, $"{ProtocolType.Traccar}");
    }

    public async Task<PositionVm> GetPositionAsync(DeviceDto deviceDto)
    {
        var url = $"api/position/{deviceDto.Name}";
        var position = await httpClientService.GetAsync<Position>(url);
        return position.MapToPositionVm(deviceDto);
    }

    public async Task<IEnumerable<PositionVm>> GetPositionAsync(IEnumerable<DeviceDto> devices)
    {
        var url = $"api/position";
        var positions = await httpClientService.GetAsync<IEnumerable<Position>>(url);
        if (positions is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Name, device => device);
        return positions.MapToPositionVm(devicesDictionary);
    }

    public async Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceDto deviceDto)
    {
        var url = $"api/position";
        var positions = await httpClientService.GetAsync<IEnumerable<Position>>(url);
        return positions is null ? ([]) : positions.MapToPositionVm(deviceDto);
    }
}
