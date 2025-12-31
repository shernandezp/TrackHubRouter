using System.Net.Http.Headers;
using System.Text;
using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces;
namespace TrackHub.Router.Infrastructure.Traccar;

// This class represents the base class for Traccar readers.
public abstract class TraccarReaderBase(
    ICredentialHttpClientFactory httpClientFactory, 
    IHttpClientService httpClientService)
{
    protected IHttpClientService HttpClientService { get; } = httpClientService;

    public ProtocolType Protocol => ProtocolType.Traccar;

    // Converts the credential to a base64-encoded string.
    private static string GetCredentialString(CredentialTokenDto credential)
    {
        var credentials = $"{credential.Username}:{credential.Password}";
        return Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
    }

    // Initializes the Traccar reader with the provided credential.
    public virtual void Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
    {
        var httpClient = httpClientFactory.CreateClientAsync(credential, cancellationToken);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetCredentialString(credential));
        HttpClientService.Init(httpClient, $"{ProtocolType.Traccar}");
    }
}
