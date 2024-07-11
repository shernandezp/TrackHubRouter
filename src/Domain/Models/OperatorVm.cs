namespace TrackHubRouter.Domain.Models;

public readonly record struct OperatorVm(
    Guid OperatorId,
    int ProtocolType,
    CredentialTokenVm? Credential
    );
