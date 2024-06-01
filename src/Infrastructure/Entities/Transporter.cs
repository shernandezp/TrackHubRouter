namespace TrackHubRouter.Infrastructure.Entities;
using Common.Infrastructure;

public sealed class Transporter(string name, short transporterTypeId, short icon) : BaseAuditableEntity
{
    public Guid TransporterId { get; private set; } = Guid.NewGuid();
    public string Name { get; set; } = name;
    public short TransporterTypeId { get; set; } = transporterTypeId;
    public short Icon { get; set; } = icon;
    public Guid? DeviceId { get; set; }
    public Device? Device { get; set; }
}
