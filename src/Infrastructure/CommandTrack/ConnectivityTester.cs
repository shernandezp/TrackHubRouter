using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.CommandTrack;

// This class represents a connectivity tester for TrackHub Router infrastructure.
public class ConnectivityTester(ICredentialHttpClientFactory httpClientFactory,
    IHttpClientService httpClientService,
    ICredentialWriter credentialWriter
    ) : CommandTrackReaderBase(httpClientFactory, httpClientService, credentialWriter), IConnectivityTester
{
    // Method to ping the TrackHub Router infrastructure.
    // It takes in a CredentialTokenDto and a CancellationToken.
    public async Task Ping(CredentialTokenDto credential, CancellationToken cancellationToken)
    {
        await Init(credential, cancellationToken);
    }
}
