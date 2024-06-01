namespace TrackHubRouter.Infrastructure.Entities;

public sealed class User(Guid userId,
    string username,
    bool active,
    Guid accountId)
{
    private Account? _account;

    public Guid UserId { get; set; } = userId;
    public string Username { get; set; } = username;
    public bool Active { get; set; } = active;
    public Guid AccountId { get; set; } = accountId;
    public ICollection<Group> Groups { get; set; } = [];

    public Account Account
    {
        get => _account ?? throw new InvalidOperationException("Account is not loaded");
        set => _account = value;
    }
}
