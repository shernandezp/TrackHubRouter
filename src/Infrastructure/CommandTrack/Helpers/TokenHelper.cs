using System.Text;
using System.Text.Json;
using TrackHubRouter.Domain.Interfaces;
using Common.Domain.Extensions;

namespace TrackHub.Router.Infrastructure.CommandTrack.Helpers;
internal class TokenHelper(ICredentialWriter credentialWriter)
{

    public async Task<string> GetTokenAsync(HttpClient httpClient,
        CredentialVm credential,
        CredentialTokenVm credentialToken,
        CancellationToken token)
    {
        return string.IsNullOrEmpty(credentialToken.Token) || IsTokenExpired(credentialToken)
            ? await RefreshTokenAsync(httpClient, credential, token)
            : credentialToken.Token;
    }

    private async Task<string> RefreshTokenAsync(HttpClient httpClient, CredentialVm credential, CancellationToken token)
    {
        var model = new AuthenticateModel
        {
            Username = credential.Username,
            Password = credential.Password,
            UniqueId = credential.Key
        };
        var response = await httpClient.PostAsync("SecurityApi/Auth/authenticate", new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"), token);
        response.EnsureSuccessStatusCode();
        var tokenResponseContent = await response.Content.ReadAsStringAsync(token);
        var newToken = tokenResponseContent.Deserialize<TokenResult>();

        if (newToken.Token is null)
        {
            throw new Exception("Failed to retrieve a new token");
        }
        await credentialWriter.UpdateTokenCredentialAsync(credential.CredentialId, new UpdateCredentialTokenDto(
                credential.CredentialId,
                newToken.Token,
                newToken.Expires,
                null,
                null
            ), token);
        return newToken.Token;
    }

    private static bool IsTokenExpired(CredentialTokenVm token)
        => DateTime.UtcNow >= token.TokenExpiration;

}
