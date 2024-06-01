using Common.Infrastructure;

namespace TrackHubRouter.Infrastructure.Entities;
public sealed class Group(long groupId, string name, string description, bool isMaster, bool active, Guid accountId) : BaseAuditableEntity
{
    private Account? _account;

    public long GroupId { get; set; } = groupId;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public bool IsMaster { get; set; } = isMaster;
    public bool Active { get; set; } = active;
    public Guid AccountId { get; set; } = accountId;
    public ICollection<User> Users { get; } = [];
    public ICollection<Device> Devices { get; } = [];

    public Account Account
    {
        get => _account ?? throw new InvalidOperationException("Account is not loaded");
        set => _account = value;
    }
}
