using Common.Domain.Enums;
using TrackHub.Router.Infrastructure.GpsGate.Models;
using TrackHubRouter.Domain.Models;

namespace TrackHub.Router.Infrastructure.GpsGate.Mappers;

internal static class DeviceMapper
{
    // Default device type and transporter type if not provided
    const DeviceType DefaultDeviceType = DeviceType.Cellular;
    const TransporterType DefaultTransporterType = TransporterType.Truck;

    // Maps a Device object and a DeviceTransporterVm object to an DeviceVm object
    public static DeviceVm MapToDeviceVm(this Device device, DeviceTransporterVm deviceDto)
        => new(
            deviceDto.TransporterId,
            device.Id,
            device.IMEI,
            device.Name,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
        );

    // Maps a Device object to an DeviceVm object with null DeviceId
    public static DeviceVm MapToDeviceVm(this Device device)
        => new(
            null,
            device.Id,
            device.IMEI,
            device.Name,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
        );

    // Maps a collection of Device objects and a dictionary of DeviceTransporterVm objects to a collection of DeviceVm objects
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Device> devices, IDictionary<int, DeviceTransporterVm> devicesDictionary)
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

    // Maps a collection of Device objects to a collection of DeviceVm objects
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Device> devices)
    {
        foreach (var device in devices)
        {
            yield return device.MapToDeviceVm();
        }
    }
}
