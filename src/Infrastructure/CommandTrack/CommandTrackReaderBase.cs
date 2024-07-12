using Ardalis.GuardClauses;
using Common.Domain.Enums;
using TrackHub.Router.Infrastructure.CommandTrack.Helpers;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Interfaces.Manager;

namespace TrackHub.Router.Infrastructure.CommandTrack;

// This class represents the base class for CommandTrack readers.
// It provides common functionality and properties for CommandTrack readers.
public class CommandTrackReaderBase
{
    private HttpClient? _httpClient;

    private readonly ICredentialHttpClientFactory _httpClientFactory;
    private readonly ICredentialWriter _credentialWriter;

    public ProtocolType Protocol => ProtocolType.CommandTrack;

    protected IHttpClientService HttpClientService { get; }
    protected IDictionary<string, string>? Header { get; private set; }

    protected CommandTrackReaderBase(ICredentialHttpClientFactory httpClientFactory,
        IHttpClientService httpClientService,
        ICredentialWriter credentialWriter)
    {
        HttpClientService = httpClientService;
        _httpClientFactory = httpClientFactory;
        _credentialWriter = credentialWriter;
    }

    // Initializes the CommandTrack reader with the provided credential.
    // It sets up the HTTP client, retrieves the access token, and initializes the HTTP client service.
    public async Task Init(CredentialTokenDto credential, CancellationToken cancellationToken)
    {
        Guard.Against.Null(credential, message: $"No CredentialToken configurations provided for {credential.CredentialId}");
        Guard.Against.Null(credential.Key, message: $"No Credential key found for {credential.CredentialId}");

        var tokenHelper = new TokenHelper(_credentialWriter);
        _httpClient = _httpClientFactory.CreateClientAsync(credential, cancellationToken);
        var token = await tokenHelper.GetTokenAsync(_httpClient, credential, cancellationToken);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        Header = new Dictionary<string, string> { { "Client-ID", credential.Key } };
        HttpClientService.Init(_httpClient, $"{ProtocolType.CommandTrack}");
    }
}
