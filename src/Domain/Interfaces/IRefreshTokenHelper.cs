namespace TrackHubRouter.Domain.Interfaces;

public interface IRefreshTokenHelper
{
    Task<string> GetTokenAsync(HttpClient httpClient, CredentialTokenVm credential, CancellationToken cancellationToken);
}
