using System.Text.Json;
using Ardalis.GuardClauses;

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

    public async Task<T?> GetAsync<T>(string url, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(_httpClient, message: $"Client configuration for {_clientName} not loaded");

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (headers is not null)
        {
            foreach (var item in headers)
            {
                request.Headers.Add(item.Key, item.Value);
            }
        }
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<T>(content);
    }
}
