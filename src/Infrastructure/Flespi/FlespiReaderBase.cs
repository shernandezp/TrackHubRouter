using System.Net.Http.Headers;
using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Flespi;

/// <summary>
/// Base class for Flespi readers providing common functionality for API communication.
/// Flespi uses FlespiToken bearer authentication.
/// </summary>
public class FlespiReaderBase
{
    private readonly ICredentialHttpClientFactory _httpClientFactory;

    protected IHttpClientService HttpClientService { get; }

    public ProtocolType Protocol => ProtocolType.Flespi;

    protected FlespiReaderBase(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService)
    {
        HttpClientService = httpClientService;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Initializes the Flespi reader with the provided credential.
    /// Sets up FlespiToken authentication header.
    /// </summary>
    public virtual void Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClientAsync(credential, cancellationToken);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("FlespiToken", credential.Token);
        HttpClientService.Init(httpClient, "Flespi");
    }
}
