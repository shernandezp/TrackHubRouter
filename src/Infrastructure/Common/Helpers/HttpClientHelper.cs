using System.Text;
using System.Text.Json;
using Common.Domain.Extensions;

namespace TrackHub.Router.Infrastructure.Common.Helpers;

// This class represents a service for making HTTP requests using HttpClient.
public class HttpClientService : IHttpClientService
{
    private HttpClient? _httpClient;
    private string? _clientName;

    public void Init(HttpClient httpClient, string clientName)
    {
        _httpClient = httpClient;
        _clientName = clientName;
    }

    /// <summary>
    /// Sends an HTTP GET request to the specified URL and returns the deserialized response content.
    /// If headers are provided, they will be added to the request.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"></param>
    /// <param name="headers"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The deserialized response content</returns>
    /// Throws an exception if the client configuration is not loaded or if the request fails.
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
        return content.Deserialize<T>();
    }

    /// <summary>
    /// Sends an HTTP POST request to the specified URL and returns the deserialized response content.
    /// If parameters are provided, they will be serialized as JSON in the request body.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"></param>
    /// <param name="parameters">Optional parameters to serialize as JSON body</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The deserialized response content</returns>
    /// Throws an exception if the client configuration is not loaded or if the request fails.
    public async Task<T?> PostAsync<T>(string url, object? parameters = null, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(_httpClient, message: $"Client configuration for {_clientName} not loaded");

        HttpContent? content = null;
        if (parameters is not null)
        {
            content = parameters is HttpContent httpContent
                ? httpContent
                : new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");
        }

        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return responseContent.Deserialize<T>();
    }
}
