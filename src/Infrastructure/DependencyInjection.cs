using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using TrackHubRouter.Infrastructure;
using TrackHubRouter.Infrastructure.Interfaces;
using TrackHubRouter.Infrastructure.Readers;
using TrackHubRouter.Infrastructure.Writers;
using TrackHubRouter.Domain.Interfaces;
using Common.Application.Interfaces;
using TrackHubRouter.Infrastructure.Identity;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.AddHeaderPropagation(o => o.Headers.Add("Authorization"));

        services.AddHttpClient("security", //Read from constants
            client =>
            {
                client.BaseAddress = new Uri("https://localhost/Security/api/"); //Read from config
                client.Timeout = TimeSpan.FromSeconds(3);   //Read from config
            })
            .AddHeaderPropagation();

        services.AddTransient<IIdentityService, IdentityService>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ICategoryWriter, CategoryWriter>();
        services.AddScoped<ICategoryReader, CategoryReader>();

        return services;
    }
}
