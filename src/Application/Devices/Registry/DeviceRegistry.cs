namespace TrackHubRouter.Application.Devices.Registry;


// This class represents a device registry that manages external device readers.
public class DeviceRegistry(IServiceScopeFactory scopeFactory) : IDeviceRegistry
{

    // Retrieves all external device readers that support the specified protocol types.
    // Parameters:
    //   types - The collection of protocol types.
    // Returns:
    //   An IEnumerable of IExternalDeviceReader representing the matching device readers.
    public IEnumerable<IExternalDeviceReader> GetReaders(IEnumerable<ProtocolType> types)
    {
        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetServices<IExternalDeviceReader>()
            .Where(reader => types.Contains(reader.Protocol));
    }

    // Retrieves the first external device reader that supports the specified protocol type.
    // Parameters:
    //   type - The protocol type.
    // Returns:
    //   An IExternalDeviceReader representing the matching device reader.
    public IExternalDeviceReader GetReader(ProtocolType type)
    {
        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetServices<IExternalDeviceReader>()
            .First(reader => reader.Protocol == type);
    }

}
