namespace TrackHubRouter.Application.Positions.Registry;

public class PositionRegistry(IServiceScopeFactory scopeFactory) : IPositionRegistry
{
    public IEnumerable<IPositionReader> GetReaders(IEnumerable<ProtocolType> types)
    {
        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetServices<IPositionReader>()
            .Where(reader => types.Contains(reader.Protocol));
    }

    public IPositionReader GetReader(ProtocolType type)
    {
        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetServices<IPositionReader>()
            .First(reader => type == reader.Protocol);
    }
}
