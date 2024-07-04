namespace TrackHubRouter.Domain.Models;

public readonly record struct TokenVm(
    string? Token,
    DateTime? TokenExpiration,
    string? RefreshToken,
    DateTime? RefreshTokenExpiration);
