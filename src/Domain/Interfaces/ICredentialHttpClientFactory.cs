namespace TrackHubRouter.Domain.Interfaces;

public interface ICredentialHttpClientFactory
{
    HttpClient CreateClientAsync(CredentialTokenVm credential, CancellationToken cancellationToken);
}
