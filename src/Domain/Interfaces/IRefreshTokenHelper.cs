namespace TrackHubRouter.Domain.Interfaces;

public interface IRefreshTokenHelper
{
    Task<string> GetTokenAsync(HttpClient httpClient, CredentialTokenDto credential, CancellationToken cancellationToken);
}
