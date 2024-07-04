namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface ICredentialReader
{
    Task<string> GetCredentialUrlAsync(Guid id, CancellationToken cancellationToken);
    Task<TokenVm> GetTokenAsync(Guid id, CancellationToken cancellationToken);
}
