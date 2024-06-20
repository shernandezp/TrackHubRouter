namespace TrackHubRouter.Domain.Models;

public readonly record struct CredentialTokenVm(
    string? Token,
    DateTime? TokenExpiration,
    string? RefreshToken,
    DateTime? RefreshTokenExpiration);
