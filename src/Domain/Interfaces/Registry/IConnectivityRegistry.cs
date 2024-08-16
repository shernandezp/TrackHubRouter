namespace TrackHubRouter.Domain.Interfaces.Registry;

public interface IConnectivityRegistry
{
    IConnectivityTester GetTester(ProtocolType type);
}
