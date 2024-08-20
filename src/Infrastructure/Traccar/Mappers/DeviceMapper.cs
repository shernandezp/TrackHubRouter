﻿namespace TrackHub.Router.Infrastructure.Traccar.Mappers;


internal static class DeviceMapper
{
    // Maps a Device object and a DeviceVm object to an ExternalDeviceVm object
    public static DeviceVm MapToDeviceVm(this Device device, DeviceOperatorVm deviceDto)
        => new(
            deviceDto.DeviceId,
            device.Id,
            device.UniqueId,
            device.Name
        );

    // Maps a Device object to an ExternalDeviceVm object with null DeviceId
    public static DeviceVm MapToDeviceVm(this Device device)
        => new(
            null,
            device.Id,
            device.UniqueId,
            device.Name
        );

    // Maps a collection of Device objects to a collection of ExternalDeviceVm objects using a dictionary of DeviceVm objects
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Device> devices, IDictionary<int, DeviceOperatorVm> devicesDictionary)
    {
        foreach (var device in devices)
        {
            if (devicesDictionary.TryGetValue(device.Id, out var selectedDevice))
            {
                yield return device.MapToDeviceVm(selectedDevice);
            }
            else
            {
                continue;
            }
        }
    }

    // Maps a collection of Device objects to a collection of ExternalDeviceVm objects
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Device> devices)
    {
        foreach (var device in devices)
        {
            yield return device.MapToDeviceVm();
        }
    }
}
