using Geotab.Checkmate.ObjectModel;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Records;

namespace TrackHub.Router.Infrastructure.Geotab;

// This class represents a connectivity tester for TrackHub Router infrastructure.
public class ConnectivityTester() : GeotabReaderBase(), IConnectivityTester
{
    // Method to ping the TrackHub Router infrastructure.
    // It takes in a CredentialTokenDto and a CancellationToken.
    public async Task Ping(CredentialTokenDto credential, CancellationToken cancellationToken)
    {
        await Init(credential, cancellationToken);
        await GeotabApi!.CallAsync<IEnumerable<Device>>("Get", typeof(Device), new { resultsLimit = 1 }, cancellationToken);
    }
}
