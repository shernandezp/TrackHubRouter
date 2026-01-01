using System.Net.Http.Headers;
using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.Samsara;

/// <summary>
/// Base class for Samsara readers providing common functionality for API communication.
/// Samsara uses Bearer token authentication.
/// </summary>
public class SamsaraReaderBase
{
    private readonly ICredentialHttpClientFactory _httpClientFactory;

    protected IHttpClientService HttpClientService { get; }

    public ProtocolType Protocol => ProtocolType.Samsara;

    protected SamsaraReaderBase(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService)
    {
        HttpClientService = httpClientService;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Initializes the Samsara reader with the provided credential.
    /// Sets up Bearer token authentication.
    /// </summary>
    public virtual void Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClientAsync(credential, cancellationToken);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credential.Token);
        HttpClientService.Init(httpClient, $"{ProtocolType.Samsara}");
    }
}
