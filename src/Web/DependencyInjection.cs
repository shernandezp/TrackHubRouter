using TrackHubRouter.Application.Devices.Registry;
using TrackHubRouter.Application.Positions.Registry;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Interfaces.Operator;
using CommandTrack = TrackHub.Router.Infrastructure.CommandTrack;
using Traccar = TrackHub.Router.Infrastructure.Traccar;

namespace TrackHubRouter.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IExternalDeviceReader, CommandTrack.DeviceReader>();
        services.AddTransient<IPositionReader, CommandTrack.PositionReader>();

        services.AddTransient<Traccar.DeviceReader>();
        services.AddTransient<Traccar.PositionReader>();
        services.AddTransient<IPositionReader, Traccar.Adapters.PositionReaderAdapter>(provider
            => new Traccar.Adapters.PositionReaderAdapter(provider.GetRequiredService<Traccar.PositionReader>()));
        services.AddTransient<IExternalDeviceReader, Traccar.Adapters.DeviceReaderAdapter>(provider 
            => new Traccar.Adapters.DeviceReaderAdapter(provider.GetRequiredService<Traccar.DeviceReader>()));

        services.AddSingleton<IDeviceRegistry, DeviceRegistry>();
        services.AddSingleton<IPositionRegistry, PositionRegistry>();

        return services;
    }
}
