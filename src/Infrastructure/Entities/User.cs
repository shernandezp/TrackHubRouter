namespace TrackHubRouter.Infrastructure.Entities;

public sealed class User(Guid userId,
    string username,
    bool active)
{
    public Guid UserId { get; set; } = userId;
    public string Username { get; set; } = username;
    public bool Active { get; set; } = active;
}
