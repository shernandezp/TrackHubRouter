using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Wialon;

/// <summary>
/// Connectivity tester for Wialon API.
/// Tests connection by performing a login operation.
/// </summary>
public sealed class ConnectivityTester(
    ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService)
    : WialonReaderBase(httpClientFactory, httpClientService), IConnectivityTester
{
    /// <summary>
    /// Tests connectivity by attempting to authenticate with the Wialon API.
    /// </summary>
    public async Task Ping(CredentialTokenDto credential, CancellationToken cancellationToken)
    {
        await Init(credential, cancellationToken);
    }
}
