namespace TrackHubRouter.Domain.Interfaces;

public interface ICredentialReader
{
    Task<CredentialVm> GetCredentialAsync(Guid id, CancellationToken cancellationToken);
    Task<string> GetCredentialUrlAsync(Guid id, CancellationToken cancellationToken);
    Task<CredentialTokenVm> GetCredentialTokenAsync(Guid id, CancellationToken cancellationToken);
}
