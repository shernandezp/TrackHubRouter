namespace TrackHubRouter.Domain.Interfaces;

public interface IHttpClientService
{
    Task<T?> GetAsync<T>(string url, IDictionary<string, string>? headers = null);
    void Init(HttpClient httpClient, string clientName);
}
