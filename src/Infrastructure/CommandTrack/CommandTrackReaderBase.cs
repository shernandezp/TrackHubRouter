using Ardalis.GuardClauses;
using Common.Domain.Enums;
using TrackHub.Router.Infrastructure.CommandTrack.Helpers;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHub.Router.Infrastructure.CommandTrack;

public class CommandTrackReaderBase
{
    private HttpClient? _httpClient;

    private readonly ICredentialHttpClientFactory _httpClientFactory;
    private readonly ICredentialWriter _credentialWriter;

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

    public async Task Init(CredentialVm credential, CredentialTokenVm? credentialToken, CancellationToken cancellationToken)
    {
        Guard.Against.Null(credentialToken, message: $"No CredentialToken configurations provided for {credential.CredentialId}");

        var tokenHelper = new TokenHelper(_credentialWriter);
        _httpClient = await _httpClientFactory.CreateClientAsync(credential.CredentialId, cancellationToken);
        var token = await tokenHelper.GetTokenAsync(_httpClient, credential, credentialToken.Value, cancellationToken);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        Header = new Dictionary<string, string> { { "Client-ID", credential.Key } };
        HttpClientService.Init(_httpClient, $"{ProtocolType.CommandTrack}");
    }
}
