using Common.Domain.Enums;
using Geotab.Checkmate.ObjectModel;
using TrackHubRouter.Domain.Models;

namespace TrackHub.Router.Infrastructure.Geotab.Mappers;

internal static class DeviceMapper
{
    // Default device type and transporter type if not provided
    const Common.Domain.Enums.DeviceType DefaultDeviceType = Common.Domain.Enums.DeviceType.Cellular;
    const TransporterType DefaultTransporterType = TransporterType.Truck;

    // Maps a Device and a DeviceVm to an DeviceVm
    public static DeviceVm MapToDeviceVm(this Device device, DeviceTransporterVm deviceDto)
        => new(
            deviceDto.TransporterId,
            (int)device.Id!.GetValue(),
            device.SerialNumber ?? string.Empty,
            device.Name ?? string.Empty,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
    );

    // Maps a Device to an DeviceVm with null DeviceId
    public static DeviceVm MapToDeviceVm(this Device device)
        => new(
            null,
            (int)device.Id!.GetValue(),
            device.SerialNumber ?? string.Empty,
            device.Name ?? string.Empty,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
    );

    // Maps a collection of DeviceVm objects to a collection of Device objects using a dictionary of DeviceTransporterVm objects
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Device> devices, IDictionary<int, DeviceTransporterVm> devicesDictionary)
    {
        foreach (var device in devices)
        {
            if (devicesDictionary.TryGetValue((int)device.Id!.GetValue(), out var selectedDevice))
            {
                yield return device.MapToDeviceVm(selectedDevice);
            }
            else
            {
                continue;
            }
        }
    }

    // Maps a collection of Device objects to a collection of DeviceVm objects
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Device> devices)
    {
        foreach (var device in devices)
        {
            yield return device.MapToDeviceVm();
        }
    }
}
