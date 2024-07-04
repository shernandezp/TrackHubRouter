namespace TrackHubRouter.Domain.Interfaces;

public interface IHttpClientService
{
    Task<T?> GetAsync<T>(string url, IDictionary<string, string>? headers = null, CancellationToken cancellationToken = default);
    void Init(HttpClient httpClient, string clientName);
}
