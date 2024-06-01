using Common.Infrastructure;

namespace TrackHubRouter.Infrastructure.Entities;
public sealed class Device(string identifier, string name, short deviceTypeId, string? description) : BaseAuditableEntity
{
    public Guid DeviceId { get; private set; } = Guid.NewGuid();
    public string Identifier { get; set; } = identifier;
    public string Name { get; set; } = name;
    public short DeviceTypeId { get; set; } = deviceTypeId;
    public string? Description { get; set; } = description;
    public Transporter? Transporter { get; set; }
    public ICollection<Group> Groups { get; set; } = [];
}
