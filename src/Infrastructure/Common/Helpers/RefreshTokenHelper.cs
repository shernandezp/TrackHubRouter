using System.Text;
using System.Text.Json;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Records;

namespace TrackHub.Router.Infrastructure.Common.Helpers;

public class RefreshTokenHelper(ICredentialWriter credentialWriter) : IRefreshTokenHelper
{

    public async Task<string> GetTokenAsync(HttpClient httpClient, CredentialTokenVm credentialToken, CredentialVm credential, CancellationToken token)
    {
        return string.IsNullOrEmpty(credentialToken.Token) || IsTokenExpired(credentialToken)
            ? await RefreshTokenAsync(httpClient, credentialToken, credential, token)
            : credentialToken.Token;
    }

    private async Task<string> RefreshTokenAsync(HttpClient httpClient, CredentialTokenVm credentialToken, CredentialVm credential, CancellationToken token)
    {
        CredentialTokenVm newCredentialToken;
        if (!string.IsNullOrEmpty(credentialToken.RefreshToken) && !IsRefreshTokenExpired(credentialToken))
        {
            // Refresh the token using the refresh token
            var refreshContent = new StringContent(JsonSerializer.Serialize(new { refreshToken = credentialToken.RefreshToken }), Encoding.UTF8, "application/json");
            var refreshResponse = await httpClient.PostAsync("refresh_token_endpoint", refreshContent, token);
            refreshResponse.EnsureSuccessStatusCode();
            var refreshResponseContent = await refreshResponse.Content.ReadAsStringAsync(token);
            newCredentialToken = JsonSerializer.Deserialize<CredentialTokenVm>(refreshResponseContent);
        }
        else
        {
            // Retrieve a new token using the credentials
            var tokenContent = new StringContent(JsonSerializer.Serialize(credential), Encoding.UTF8, "application/json");
            var tokenResponse = await httpClient.PostAsync("token_endpoint", tokenContent, token);
            tokenResponse.EnsureSuccessStatusCode();
            var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync(token);
            newCredentialToken = JsonSerializer.Deserialize<CredentialTokenVm>(tokenResponseContent);
        }

        if (newCredentialToken.Token is null)
        {
            throw new Exception("Failed to retrieve a new token");
        }
        await credentialWriter.UpdateTokenCredentialAsync(credential.CredentialId, new UpdateCredentialTokenDto(
                credential.CredentialId,
                newCredentialToken.Token,
                newCredentialToken.TokenExpiration,
                newCredentialToken.RefreshToken,
                newCredentialToken.RefreshTokenExpiration
            ), token);
        return newCredentialToken.Token;
    }

    private static bool IsTokenExpired(CredentialTokenVm token)
        => DateTime.UtcNow >= token.TokenExpiration;

    private static bool IsRefreshTokenExpired(CredentialTokenVm token)
        => DateTime.UtcNow >= token.RefreshTokenExpiration;
}
