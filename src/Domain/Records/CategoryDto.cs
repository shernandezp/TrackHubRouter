namespace TrackHubRouter.Domain.Records;

public record struct CategoryDto(
    Guid CategoryId,
    string Name,
    string? Description,
    CategoryType Type);
