using TrackHubRouter.Domain.Records;

namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface ICredentialWriter
{
    Task<bool> UpdateTokenAsync(Guid id, UpdateTokenDto credential, CancellationToken token);
}
