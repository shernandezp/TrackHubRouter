using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHubRouter.Domain.Interfaces.Registry;

public interface IPositionRegistry
{
    IEnumerable<IPositionReader> GetReaders(IEnumerable<ProtocolType> types);
}
