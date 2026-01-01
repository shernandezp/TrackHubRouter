namespace TrackHub.Router.Infrastructure.Wialon.Models;

/// <summary>
/// Response from Wialon core/search_items API
/// </summary>
internal sealed record SearchResponse(
    IEnumerable<Unit>? Items,
    int TotalItemsCount
);

/// <summary>
/// Response from Wialon core/search_item API (single item)
/// </summary>
internal sealed record SingleItemResponse(
    Unit? Item
);
