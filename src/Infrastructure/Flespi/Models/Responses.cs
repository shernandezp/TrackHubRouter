namespace TrackHub.Router.Infrastructure.Flespi.Models;

/// <summary>
/// Response from Flespi /gw/devices API
/// </summary>
internal sealed record DeviceListResponse(
    List<Device>? Result
);

/// <summary>
/// Response from Flespi /gw/devices/{id}/messages API
/// </summary>
internal sealed record MessageListResponse(
    IEnumerable<Message>? Result
);
