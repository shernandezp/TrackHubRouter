namespace TrackHubRouter.Domain.Models;

public readonly record struct DeviceOperatorVm(
    Guid DeviceId,
    int Identifier,
    string Serial,
    string Name
    );
