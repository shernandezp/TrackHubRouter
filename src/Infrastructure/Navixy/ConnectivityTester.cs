using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Navixy;

/// <summary>
/// Connectivity tester for Navixy API.
/// Tests connection by authenticating and fetching tracker list.
/// </summary>
public sealed class ConnectivityTester(
    ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService)
    : NavixyReaderBase(httpClientFactory, httpClientService), IConnectivityTester
{
    /// <summary>
    /// Tests connectivity by attempting to authenticate with the Navixy API.
    /// </summary>
    public async Task Ping(CredentialTokenDto credential, CancellationToken cancellationToken)
    {
        await Init(credential, cancellationToken);
        // Make a simple API call to verify the session hash is valid
        await HttpClientService.PostAsync<TrackerListResponse>(
            $"{BaseUrl}/v2/tracker/list", new { hash = Hash }, cancellationToken);
    }
}
