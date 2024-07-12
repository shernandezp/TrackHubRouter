using Common.Domain.Extensions;
using TrackHub.Router.Infrastructure.Traccar.Mappers;
using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Traccar;

public sealed class PositionReader(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService) 
    : TraccarReaderBase(httpClientFactory, httpClientService)
{

    public async Task<PositionVm> GetDevicePositionAsync(DeviceVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"api/positions?id={deviceDto.Identifier}";
        var position = await HttpClientService.GetAsync<Position>(url, cancellationToken: cancellationToken);
        return position.MapToPositionVm(deviceDto);
    }

    public async Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceVm> devices, CancellationToken cancellationToken)
    {
        var url = $"api/positions{devices.GetIdsQueryString()}";
        var positions = await HttpClientService.GetAsync<IEnumerable<Position>>(url, cancellationToken: cancellationToken);
        if (positions is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return positions.MapToPositionVm(devicesDictionary);
    }

    public async Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"api/positions?deviceId={deviceDto.Identifier}&from={from.ToIso8601String()}&to={to.ToIso8601String()}";
        var positions = await HttpClientService.GetAsync<IEnumerable<Position>>(url, cancellationToken: cancellationToken);
        return positions is null ? ([]) : positions.MapToPositionVm(deviceDto);
    }
}
