namespace CommandTrack;

using Common.Domain.Enums;

public sealed class PositionReader(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(ProtocolType.CommandTrack.ToString());

    public async Task<Position> GetPositionAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/positions/{id}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Position>(content);
    }
}
