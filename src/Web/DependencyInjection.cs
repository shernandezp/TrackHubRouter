using TrackHubRouter.Application.Positions.Registry;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHubRouter.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {

        services.AddTransient<IPositionReader, TrackHub.Router.Infrastructure.CommandTrack.PositionReader>();
        services.AddTransient<IPositionReader, TrackHub.Router.Infrastructure.Traccar.PositionReader>();

        services.AddSingleton<IPositionRegistry, PositionRegistry>();

        return services;
    }
}
