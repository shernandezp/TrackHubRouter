namespace TrackHubRouter.Domain.Interfaces;

public interface IRefreshTokenHelper
{
    Task<string> GetTokenAsync(HttpClient httpClient, CredentialTokenVm credentialToken, CredentialVm credential, CancellationToken token);
}
