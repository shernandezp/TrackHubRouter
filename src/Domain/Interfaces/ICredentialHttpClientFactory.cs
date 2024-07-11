namespace TrackHubRouter.Domain.Interfaces;

public interface ICredentialHttpClientFactory
{
    HttpClient CreateClientAsync(CredentialTokenDto credential, CancellationToken cancellationToken);
}
