using System.Text;
using System.Text.Json;
using TrackHubRouter.Domain.Models;

namespace TrackHub.Router.Infrastructure.Common;

public class TokenService
{
    private CredentialTokenVm _currentToken;
    private readonly CredentialVm _credentials;
    private readonly HttpClient _httpClient;

    public TokenService(CredentialVm credentials, HttpClient httpClient)
    {
        _credentials = credentials;
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string> GetTokenAsync()
    {
        if (string.IsNullOrEmpty(_currentToken.Token) || IsTokenExpired(_currentToken))
        {
            await RefreshTokenAsync();
        }

        return _currentToken.Token!;
    }

    private async Task RefreshTokenAsync()
    {
        if (!string.IsNullOrEmpty(_currentToken.RefreshToken) && !IsRefreshTokenExpired(_currentToken))
        {
            // Refresh the token using the refresh token
            var refreshContent = new StringContent(JsonSerializer.Serialize(new { refreshToken = _currentToken.RefreshToken }), Encoding.UTF8, "application/json");
            var refreshResponse = await _httpClient.PostAsync("your_refresh_token_endpoint", refreshContent);
            refreshResponse.EnsureSuccessStatusCode();
            var refreshResponseContent = await refreshResponse.Content.ReadAsStringAsync();
            _currentToken = JsonSerializer.Deserialize<CredentialTokenVm>(refreshResponseContent);
        }
        else
        {
            // Retrieve a new token using the credentials
            var tokenContent = new StringContent(JsonSerializer.Serialize(_credentials), Encoding.UTF8, "application/json");
            var tokenResponse = await _httpClient.PostAsync("your_token_endpoint", tokenContent);
            tokenResponse.EnsureSuccessStatusCode();
            var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync();
            _currentToken = JsonSerializer.Deserialize<CredentialTokenVm>(tokenResponseContent);
        }
    }

    private static bool IsTokenExpired(CredentialTokenVm token)
        => DateTime.UtcNow >= token.TokenExpiration;

    private static bool IsRefreshTokenExpired(CredentialTokenVm token)
        => DateTime.UtcNow >= token.RefreshTokenExpiration;
}
