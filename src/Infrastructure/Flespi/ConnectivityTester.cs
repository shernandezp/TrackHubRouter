using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Flespi;

/// <summary>
/// Connectivity tester for Flespi API.
/// Tests connection by fetching device list.
/// </summary>
public sealed class ConnectivityTester(
    ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService)
    : FlespiReaderBase(httpClientFactory, httpClientService), IConnectivityTester
{
    /// <summary>
    /// Tests connectivity by attempting to fetch devices from the Flespi API.
    /// </summary>
    public async Task Ping(CredentialTokenDto credential, CancellationToken cancellationToken)
    {
        Init(credential, cancellationToken);
        // Make a simple API call to verify connectivity
        var url = "gw/devices/all";
        await HttpClientService.GetAsync<DeviceListResponse>(url, cancellationToken: cancellationToken);
    }
}
