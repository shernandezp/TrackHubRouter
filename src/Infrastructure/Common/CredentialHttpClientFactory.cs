using System.Collections.Concurrent;
using TrackHubRouter.Domain.Interfaces;

namespace Common;

public sealed class CredentialHttpClientFactory(IHttpClientFactory httpClientFactory, ICredentialReader credentialReader) : ICredentialHttpClientFactory
{
    private readonly ConcurrentDictionary<string, string> _baseURLs = new();

    public async Task<HttpClient> CreateClientAsync(string name, CancellationToken cancellationToken)
    {
        if (!_baseURLs.TryGetValue(name, out var baseUrl))
        {
            baseUrl = await credentialReader.GetCredentialUrlAsync(Guid.Parse(name), cancellationToken);
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _baseURLs[name] = baseUrl;
            }
        }

        var httpClient = httpClientFactory.CreateClient(name);
        if (!string.IsNullOrEmpty(baseUrl))
        {
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
        else
        {
            throw new InvalidOperationException($"Base URL for client '{name}' not initialized.");
        }
        return httpClient;
    }
}
