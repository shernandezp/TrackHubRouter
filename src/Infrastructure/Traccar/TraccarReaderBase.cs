using System.Net.Http.Headers;
using System.Text;
using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces;
namespace TrackHub.Router.Infrastructure.Traccar;

public class TraccarReaderBase
{
    private readonly ICredentialHttpClientFactory _httpClientFactory;
    protected IHttpClientService HttpClientService { get; }
    
    public ProtocolType Protocol => ProtocolType.Traccar;

    protected TraccarReaderBase(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService)
    {
        HttpClientService = httpClientService;
        _httpClientFactory = httpClientFactory;
    }

    private static string GetCredentialString(CredentialTokenVm credential)
    {
        var credentials = $"{credential.Username}:{credential.Password}";
        return Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
    }

    public void Init(CredentialTokenVm credential, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClientAsync(credential, cancellationToken);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetCredentialString(credential));
        HttpClientService.Init(httpClient, $"{ProtocolType.Traccar}");
    }
}
