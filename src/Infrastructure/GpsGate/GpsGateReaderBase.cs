using System.Net.Http.Headers;
using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Records;

namespace TrackHub.Router.Infrastructure.GpsGate;

public class GpsGateReaderBase
{
    private readonly ICredentialHttpClientFactory _httpClientFactory;
    protected IHttpClientService HttpClientService { get; }

    public ProtocolType Protocol => ProtocolType.GpsGate;
    protected string ApplicationId = string.Empty;
    protected string UserId = string.Empty;

    protected GpsGateReaderBase(ICredentialHttpClientFactory httpClientFactory, IHttpClientService httpClientService)
    {
        HttpClientService = httpClientService;
        _httpClientFactory = httpClientFactory;
    }

    // Initializes the GpsGate reader with the provided credential.
    public virtual void Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClientAsync(credential, cancellationToken);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("Authorization", credential.Password);
        HttpClientService.Init(httpClient, $"{ProtocolType.GpsGate}");
        ApplicationId = credential.Key ?? ApplicationId;
        UserId = credential.Key2 ?? UserId;
    }
}
