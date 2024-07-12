using TrackHubRouter.Domain.Records;

namespace TrackHub.Router.Infrastructure.Common;

// This class represents a factory for creating HttpClient instances with credentials.
public sealed class CredentialHttpClientFactory(IHttpClientFactory httpClientFactory) : ICredentialHttpClientFactory
{
    // Creates a new HttpClient instance with the specified credential and cancellation token.
    // If the credential has a valid URI, sets the BaseAddress and Timeout properties of the HttpClient.
    // Otherwise, throws an InvalidOperationException.
    public HttpClient CreateClientAsync(CredentialTokenDto credential, CancellationToken cancellationToken)
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
