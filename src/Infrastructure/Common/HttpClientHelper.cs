using System.Text.Json;
using Ardalis.GuardClauses;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Common;
public class HttpClientService : IHttpClientService
{
    private HttpClient? _httpClient;
    private string? _clientName;

    public void Init(HttpClient httpClient, string clientName)
    {
        _httpClient = httpClient;
        _clientName = clientName;
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        Guard.Against.Null(_httpClient, message: $"Client configuration for {_clientName} not loaded");

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content);
    }
}
