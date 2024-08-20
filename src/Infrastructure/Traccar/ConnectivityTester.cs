using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Traccar;

// This class represents a connectivity tester for Traccar.
public sealed class ConnectivityTester(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService)
    : TraccarReaderBase(httpClientFactory, httpClientService), IConnectivityTester
{
    // Sends a ping request to the Traccar server.
    public async Task Ping(CredentialTokenDto credential, CancellationToken cancellationToken)
    {
        Init(credential, cancellationToken);
        var url = $"api/server";
        await HttpClientService.GetAsync<Pong>(url, cancellationToken: cancellationToken);
    }
    private class Pong { }
}
