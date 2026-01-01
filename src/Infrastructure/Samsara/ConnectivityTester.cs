using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Samsara;

/// <summary>
/// Connectivity tester for Samsara API.
/// Tests connection by making a simple API call.
/// </summary>
public sealed class ConnectivityTester(
    ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService)
    : SamsaraReaderBase(httpClientFactory, httpClientService), IConnectivityTester
{
    /// <summary>
    /// Tests connectivity by attempting to fetch vehicle stats from the Samsara API.
    /// </summary>
    public async Task Ping(CredentialTokenDto credential, CancellationToken cancellationToken)
    {
        Init(credential, cancellationToken);
        // Make a simple API call to verify connectivity
        var url = "fleet/vehicles/stats?limit=1";
        await HttpClientService.GetAsync<VehicleStatsResponse>(url, cancellationToken: cancellationToken);
    }
}
