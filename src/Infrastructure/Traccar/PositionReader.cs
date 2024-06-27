using Common.Domain.Extensions;
using TrackHub.Router.Infrastructure.Traccar.Mappers;
using TrackHubRouter.Domain.Extensions;
using TrackHub.Router.Infrastructure.Common;
using TrackHubRouter.Domain.Interfaces;
using Common.Domain.Enums;
using System.Text;
using System.Net.Http.Headers;

namespace TrackHub.Router.Infrastructure.Traccar;

public sealed class PositionReader(CredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService) : IPositionReader
{
    private HttpClient? _httpClient;

    private static string GetCredentialString(CredentialVm credential)
    {
        var credentials = $"{credential.Username}:{credential.Password}";
        return Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
    }

    public async Task Init(CredentialVm credential, CancellationToken cancellationToken)
    {
        _httpClient = await httpClientFactory.CreateClientAsync(credential.CredentialId, cancellationToken);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetCredentialString(credential));
        httpClientService.Init(_httpClient, $"{ProtocolType.Traccar}");
    }

    public async Task<PositionVm> GetDevicePositionAsync(DeviceDto deviceDto)
    {
        var url = $"positions?id={deviceDto.Identifier}";
        var position = await httpClientService.GetAsync<Position>(url);
        return position.MapToPositionVm(deviceDto);
    }

    public async Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceDto> devices)
    {
        var url = $"positions{devices.GetIdsQueryString()}";
        var positions = await httpClientService.GetAsync<IEnumerable<Position>>(url);
        if (positions is null)
        {
            return [];
        }
        var devicesDictionary = devices.ToDictionary(device => device.Identifier, device => device);
        return positions.MapToPositionVm(devicesDictionary);
    }

    public async Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceDto deviceDto)
    {
        var url = $"positions?deviceId={deviceDto.Identifier}&from={from.ToIso8601String()}&to={to.ToIso8601String()}";
        var positions = await httpClientService.GetAsync<IEnumerable<Position>>(url);
        return positions is null ? ([]) : positions.MapToPositionVm(deviceDto);
    }
}
