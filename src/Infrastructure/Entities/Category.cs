using Common.Domain.Enums;
using Common.Infrastructure;

namespace TrackHubRouter.Infrastructure.Entities;

public sealed class Category(string name, 
        string? description, 
        CategoryType type) : BaseAuditableEntity
{
    public Guid CategoryId { get; private set; } = Guid.NewGuid();
    public string Name { get; set; } = name;
    public string? Description { get; set; } = description;
    public CategoryType Type { get; set; } = type;
    public bool Active { get; set; } = true;
}
