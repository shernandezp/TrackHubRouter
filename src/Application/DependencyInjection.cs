using System.Reflection;
using Common.Application;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddApplicationServices(assembly);
        services.AddDistributedMemoryCache();
        return services;
    }
}
