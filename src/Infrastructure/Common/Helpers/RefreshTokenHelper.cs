using System.Text;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Records;

namespace TrackHub.Router.Infrastructure.Common.Helpers;

public class RefreshTokenHelper(ICredentialWriter credentialWriter) : IRefreshTokenHelper
{

    public async Task<string> GetTokenAsync(HttpClient httpClient, CredentialTokenDto credential, CancellationToken token)
    {
        return string.IsNullOrEmpty(credential.Token) || IsTokenExpired(credential)
            ? await RefreshTokenAsync(httpClient, credential, token)
            : credential.Token;
    }

    private async Task<string> RefreshTokenAsync(HttpClient httpClient, CredentialTokenDto credential, CancellationToken token)
    {
        TokenVm newCredentialToken;
        if (!string.IsNullOrEmpty(credential.RefreshToken) && !IsRefreshTokenExpired(credential))
        {
            // Refresh the token using the refresh token
            var refreshContent = new StringContent(JsonSerializer.Serialize(new { refreshToken = credential.RefreshToken }), Encoding.UTF8, "application/json");
            var refreshResponse = await httpClient.PostAsync("refresh_token_endpoint", refreshContent, token);
            refreshResponse.EnsureSuccessStatusCode();
            var refreshResponseContent = await refreshResponse.Content.ReadAsStringAsync(token);
            newCredentialToken = JsonSerializer.Deserialize<TokenVm>(refreshResponseContent);
        }
        else
        {
            // Retrieve a new token using the credentials
            var tokenContent = new StringContent(JsonSerializer.Serialize(credential), Encoding.UTF8, "application/json");
            var tokenResponse = await httpClient.PostAsync("token_endpoint", tokenContent, token);
            tokenResponse.EnsureSuccessStatusCode();
            var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync(token);
            newCredentialToken = JsonSerializer.Deserialize<TokenVm>(tokenResponseContent);
        }

        if (newCredentialToken.Token is null)
        {
            throw new Exception("Failed to retrieve a new token");
        }
        await credentialWriter.UpdateTokenAsync(credential.CredentialId, new UpdateTokenDto(
                credential.CredentialId,
                newCredentialToken.Token,
                newCredentialToken.TokenExpiration,
                newCredentialToken.RefreshToken,
                newCredentialToken.RefreshTokenExpiration
            ), token);
        return newCredentialToken.Token;
    }

    private static bool IsTokenExpired(CredentialTokenDto token)
        => DateTime.UtcNow >= token.TokenExpiration;

    private static bool IsRefreshTokenExpired(CredentialTokenDto token)
        => DateTime.UtcNow >= token.RefreshTokenExpiration;
}
