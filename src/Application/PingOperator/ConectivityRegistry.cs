namespace TrackHubRouter.Application.PingOperator;

// This class represents a registry for connectivity testers.
public class ConnectivityRegistry(IServiceScopeFactory scopeFactory) : IConnectivityRegistry
{
    // Get the connectivity tester for the specified protocol type.
    // Parameters:
    //   type: The protocol type.
    // Returns:
    //   The connectivity tester.
    public IConnectivityTester GetTester(ProtocolType type)
    {
        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetServices<IConnectivityTester>()
            .First(reader => reader.Protocol == type);
    }

}
