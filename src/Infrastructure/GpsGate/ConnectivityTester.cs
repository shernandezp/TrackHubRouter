using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Records;

namespace TrackHub.Router.Infrastructure.GpsGate;

// This class represents a connectivity tester for GpsGate.
public sealed class ConnectivityTester(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService)
    : GpsGateReaderBase(httpClientFactory, httpClientService), IConnectivityTester
{
    // Sends a ping request to the GpsGate server.
    public async Task Ping(CredentialTokenDto credential, CancellationToken cancellationToken)
    {
        Init(credential, cancellationToken);
        var url = $"api/v.1/applications";
        await HttpClientService.GetAsync<Pong>(url, cancellationToken: cancellationToken);
    }
    private class Pong { }
}
