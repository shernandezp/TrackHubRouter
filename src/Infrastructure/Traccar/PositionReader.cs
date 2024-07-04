using Common.Domain.Extensions;
using TrackHub.Router.Infrastructure.Traccar.Mappers;
using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.Traccar;

public sealed class PositionReader(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService) 
    : TraccarReaderBase(httpClientFactory, httpClientService), IPositionReader
{

    public async Task<PositionVm> GetDevicePositionAsync(DeviceDto deviceDto, CancellationToken cancellationToken)
    {
        var url = $"/positions?id={deviceDto.Identifier}";
        var position = await HttpClientService.GetAsync<Position>(url, cancellationToken: cancellationToken);
        return position.MapToPositionVm(deviceDto);
    }

    public async Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceDto> devices, CancellationToken cancellationToken)
    {
        var url = $"/positions{devices.GetIdsQueryString()}";
        var positions = await HttpClientService.GetAsync<IEnumerable<Position>>(url, cancellationToken: cancellationToken);
        if (positions is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return positions.MapToPositionVm(devicesDictionary);
    }

    public async Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceDto deviceDto, CancellationToken cancellationToken)
    {
        var url = $"/positions?deviceId={deviceDto.Identifier}&from={from.ToIso8601String()}&to={to.ToIso8601String()}";
        var positions = await HttpClientService.GetAsync<IEnumerable<Position>>(url, cancellationToken: cancellationToken);
        return positions is null ? ([]) : positions.MapToPositionVm(deviceDto);
    }
}
