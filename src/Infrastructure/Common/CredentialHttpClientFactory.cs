using TrackHubRouter.Domain.Models;

namespace TrackHub.Router.Infrastructure.Common;

public sealed class CredentialHttpClientFactory(IHttpClientFactory httpClientFactory) : ICredentialHttpClientFactory
{
    public HttpClient CreateClientAsync(CredentialTokenVm credential, CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(credential.CredentialId.ToString());
        if (!string.IsNullOrEmpty(credential.Uri))
        {
            httpClient.BaseAddress = new Uri(credential.Uri);
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
        else
        {
            throw new InvalidOperationException($"Base URL for client '{credential.CredentialId}' not initialized.");
        }
        return httpClient;
    }
}
