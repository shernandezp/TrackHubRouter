using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using TrackHubRouter.Infrastructure;
using TrackHubRouter.Infrastructure.Interfaces;
using TrackHubRouter.Infrastructure.Readers;
using TrackHubRouter.Infrastructure.Writers;
using TrackHubRouter.Domain.Interfaces;
using Common.Application.Interfaces;
using TrackHubRouter.Infrastructure.Identity;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;

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

        //Header propagation for REST client
        services.AddHttpClient("security", //Read from constants
            client =>
            {
                var url = configuration.GetValue<string>("AppSettings:RESTIdentityService");
#pragma warning disable CS8604 // Possible null reference argument.
                client.BaseAddress = new Uri(url);
#pragma warning restore CS8604 // Possible null reference argument.
                client.Timeout = TimeSpan.FromSeconds(10);   //Read from config
            })
            .AddHeaderPropagation();

        //Header propagation for GraphQL client
        services.AddSingleton<IGraphQLClient>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("security");
            var url = configuration.GetValue<string>("AppSettings:GraphQLIdentityService");
#pragma warning disable CS8604 // Possible null reference argument.
            var options = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(url)
            };
#pragma warning restore CS8604 // Possible null reference argument.
            var jsonSerializer = new SystemTextJsonSerializer();
            return new GraphQLHttpClient(options, jsonSerializer, httpClient);
        });

        services.AddTransient<IIdentityService, IdentityService>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ICategoryWriter, CategoryWriter>();
        services.AddScoped<ICategoryReader, CategoryReader>();

        return services;
    }
}
