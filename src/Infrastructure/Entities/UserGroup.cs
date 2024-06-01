namespace TrackHubRouter.Infrastructure.Entities;
public sealed class UserGroup
{
    private Group? _group;
    private User? _user;

    public required Guid UserId { get; set; }
    public required long GroupId { get; set; }

    public Group Group
    {
        get => _group ?? throw new InvalidOperationException("Group is not loaded");
        set => _group = value;
    }
    public User User
    {
        get => _user ?? throw new InvalidOperationException("User is not loaded");
        set => _user = value;
    }
}
