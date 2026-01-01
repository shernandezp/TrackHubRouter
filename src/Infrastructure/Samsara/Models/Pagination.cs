namespace TrackHub.Router.Infrastructure.Samsara.Models;

/// <summary>
/// Samsara pagination model
/// </summary>
internal readonly record struct Pagination(
    string? EndCursor,
    bool HasNextPage
);
