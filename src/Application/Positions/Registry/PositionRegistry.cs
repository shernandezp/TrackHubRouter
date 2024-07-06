namespace TrackHubRouter.Application.Positions.Registry;

public class PositionRegistry(IServiceProvider serviceProvider) : IPositionRegistry
{
    public IEnumerable<IPositionReader> GetReaders(IEnumerable<ProtocolType> types)
    {
        return serviceProvider.GetServices<IPositionReader>()
            .Where(reader => types.Contains(reader.Protocol));
    }
}
