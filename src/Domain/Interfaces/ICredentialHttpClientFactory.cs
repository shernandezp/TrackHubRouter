namespace TrackHubRouter.Domain.Interfaces;

public interface ICredentialHttpClientFactory
{
    Task<HttpClient> CreateClientAsync(Guid name, CancellationToken cancellationToken);
}
