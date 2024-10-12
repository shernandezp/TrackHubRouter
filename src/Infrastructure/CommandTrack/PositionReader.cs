namespace TrackHub.Router.Infrastructure.CommandTrack;

using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Models;
using TrackHub.Router.Infrastructure.CommandTrack.Mappers;
using Common.Domain.Extensions;
using TrackHubRouter.Domain.Extensions;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Interfaces.Operator;

public sealed class PositionReader(ICredentialHttpClientFactory httpClientFactory, 
    IHttpClientService httpClientService,
    ICredentialWriter credentialWriter
    ) : CommandTrackReaderBase(httpClientFactory, httpClientService, credentialWriter), IPositionReader
{
    public async Task<PositionVm> GetDevicePositionAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"DataConnectAPI/api/Device/{deviceDto.Name}";
        var position = await HttpClientService.GetAsync<DevicePosition>(url, Header, cancellationToken);
        return position.MapToPositionVm(deviceDto);
    }

    public async Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
    {
        var url = $"DataConnectAPI/api/Devices?{devices.GetIdsQueryString()}";
        var positions = await HttpClientService.GetAsync<IEnumerable<DevicePosition>>(url, Header, cancellationToken);
        if (positions is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Name, device => device);
        return positions.MapToPositionVm(devicesDictionary);
    }

    public async Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
    {
        var url = $"DataConnectAPI/api/Position/{deviceDto.Name}/{from.ToIso8601String()}/{to.ToIso8601String()}";
        var positions = await HttpClientService.GetAsync<IEnumerable<Position>>(url, Header, cancellationToken);
        return positions is null ? ([]) : positions.MapToPositionVm(deviceDto);
    }
}
