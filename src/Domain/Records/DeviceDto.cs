namespace TrackHubRouter.Domain.Records;

public readonly record struct DeviceDto(
    Guid DeviceId,
    int Identifier,
    string Serial,
    string Name
    );
