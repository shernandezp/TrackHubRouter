namespace TrackHubRouter.Domain.Interfaces;

public interface IHttpClientService
{
    Task<T?> GetAsync<T>(string url);
    void Init(HttpClient httpClient, string clientName);
}
