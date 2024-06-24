namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCommandTrackContext(this IServiceCollection services)
    {
        services.AddScoped<IPositionReader, PositionReader>();
        return services;
    }
}
