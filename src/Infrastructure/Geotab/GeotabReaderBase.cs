using Common.Domain.Enums;
using Geotab.Checkmate;
using TrackHubRouter.Domain.Records;

namespace TrackHub.Router.Infrastructure.Geotab;

// This class represents the base class for Geotab readers.
public abstract class GeotabReaderBase
{
    protected API? GeotabApi = null;

    public ProtocolType Protocol => ProtocolType.GeoTab;

    // Initializes the Geotab reader with the provided credential.
    public async Task Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
    {
        GeotabApi = new API(credential.Username, credential.Password, null, credential.Key!);
        await GeotabApi.AuthenticateAsync(cancellationToken);
    }
}
