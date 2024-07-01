using TrackHubRouter.Domain.Records;

namespace TrackHubRouter.Domain.Interfaces;

public interface ICredentialWriter
{
    Task<bool> UpdateTokenCredentialAsync(Guid id, UpdateCredentialTokenDto credential, CancellationToken token);
}
