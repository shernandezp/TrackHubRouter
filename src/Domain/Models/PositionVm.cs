﻿namespace TrackHubRouter.Domain.Models;
public readonly record struct PositionVm(
    Guid DeviceId,
    double Latitude,
    double Longitude,
    double Altitude,
    DateTimeOffset DeviceDateTime,
    DateTimeOffset? ServerDateTime,
    double Speed,
    double Course,
    int? EventId,
    string? Address,
    string? City,
    string? State,
    string? Country,
    AttributesVm? Attributes
    );