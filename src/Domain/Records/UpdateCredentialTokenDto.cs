namespace TrackHubRouter.Domain.Records;

public readonly record struct UpdateCredentialTokenDto(
    Guid CredentialId,
    string? Token,
    DateTime? TokenExpiration,
    string? RefreshToken,
    DateTime? RefreshTokenExpiration
    );
