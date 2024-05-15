using Common.Application.Interfaces;

namespace TrackHubRouter.Infrastructure.Identity;
public class IdentityService(IHttpClientFactory httpClientFactory) : IIdentityService
{
    private readonly HttpClient _securityClient = httpClientFactory.CreateClient("security");

    private async Task<T> GetAsync<T>(string url, CancellationToken token)
    {
        var response = await _securityClient.GetAsync(url, token);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync(token);
        return (T)Convert.ChangeType(result, typeof(T));
    }

    public Task<bool> AuthorizeAsync(Guid userId, string policyName, CancellationToken token)
    {
        string url = $"Identity/Authorize/{userId}/{policyName}";
        return GetAsync<bool>(url, token);
    }

    public Task<string> GetUserNameAsync(Guid userId, CancellationToken token)
    {
        string url = $"Identity/UserName/{userId}";
        return GetAsync<string>(url, token);
    }

    public Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken token)
    {
        string url = $"Identity/IsInRole/{userId}/{role}";
        return GetAsync<bool>(url, token);
    }
}
