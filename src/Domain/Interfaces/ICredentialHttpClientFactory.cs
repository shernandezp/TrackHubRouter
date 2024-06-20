namespace TrackHubRouter.Domain.Interfaces;

public interface ICredentialHttpClientFactory
{
    Task<HttpClient> CreateClientAsync(string name, CancellationToken cancellationToken);
}
