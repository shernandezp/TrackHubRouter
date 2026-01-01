namespace TrackHub.Router.Infrastructure.Samsara.Models;

/// <summary>
/// Response from Samsara /fleet/vehicles/stats API
/// </summary>
internal sealed record VehicleStatsResponse(
    IEnumerable<VehicleStats>? Data,
    Pagination? Pagination
);

/// <summary>
/// Response from Samsara /fleet/vehicles/stats/history API
/// </summary>
internal sealed record VehicleHistoryResponse(
    IEnumerable<VehicleHistory>? Data,
    Pagination? Pagination
);
