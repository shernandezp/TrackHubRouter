namespace TrackHubRouter.Application.Devices.Registry;

public class DeviceRegistry(IServiceProvider serviceProvider) : IDeviceRegistry
{
    public IEnumerable<IExternalDeviceReader> GetReaders(IEnumerable<ProtocolType> types)
        => serviceProvider.GetServices<IExternalDeviceReader>()
            .Where(reader => types.Contains(reader.Protocol));

    public IExternalDeviceReader GetReader(ProtocolType type)
        => serviceProvider.GetServices<IExternalDeviceReader>()
            .First(reader => reader.Protocol == type);

}
