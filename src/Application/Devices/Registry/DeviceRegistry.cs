namespace TrackHubRouter.Application.Devices.Registry;

public class DeviceRegistry(IServiceScopeFactory scopeFactory) : IDeviceRegistry
{

    public IEnumerable<IExternalDeviceReader> GetReaders(IEnumerable<ProtocolType> types)
    {
        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetServices<IExternalDeviceReader>()
            .Where(reader => types.Contains(reader.Protocol));
    }

    public IExternalDeviceReader GetReader(ProtocolType type)
    {
        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetServices<IExternalDeviceReader>()
            .First(reader => reader.Protocol == type); 
    }

}
