namespace TrackHubRouter.Domain.Models;

public readonly record struct OperatorVm(
    Guid OperatorId,
    int ProtocolTypeId,
    Guid AccountId,
    CredentialTokenVm? Credential
    );
