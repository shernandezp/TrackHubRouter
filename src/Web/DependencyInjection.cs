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
        services.AddScoped<IExternalDeviceReader, CommandTrack.DeviceReader>();
        services.AddScoped<IPositionReader, CommandTrack.PositionReader>();

        services.AddScoped<Traccar.DeviceReader>();
        services.AddScoped<Traccar.PositionReader>();
        services.AddScoped<IPositionReader, Traccar.Adapters.PositionReaderAdapter>(provider
            => new Traccar.Adapters.PositionReaderAdapter(provider.GetRequiredService<Traccar.PositionReader>()));
        services.AddScoped<IExternalDeviceReader, Traccar.Adapters.DeviceReaderAdapter>(provider 
            => new Traccar.Adapters.DeviceReaderAdapter(provider.GetRequiredService<Traccar.DeviceReader>()));

        services.AddSingleton<IDeviceRegistry, DeviceRegistry>();
        services.AddSingleton<IPositionRegistry, PositionRegistry>();

        return services;
    }
}
