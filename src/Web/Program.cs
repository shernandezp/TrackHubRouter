using System.Reflection;
using Common.Application;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using TrackHubRouter.Infrastructure;
using TrackHubRouter.Web.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var assembly = Assembly.GetExecutingAssembly();
builder.Services.AddApplicationServices(assembly);
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices("Router API");

// Add HealthChecks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

builder.Services.AddHttpClient("GraphQLClient")
    .AddHeaderPropagation();

builder.Services.AddSingleton<IGraphQLClient>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("GraphQLClient");
    var options = new GraphQLHttpClientOptions
    {
        EndPoint = new Uri("https://localhost/Security/graphql/"), //Add this endpoint to the configuration file
    };
    var jsonSerializer = new SystemTextJsonSerializer();
    return new GraphQLHttpClient(options, jsonSerializer, httpClient);
});

builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Categories>();

var app = builder.Build();

app.UseHeaderPropagation();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

app.UseExceptionHandler(options => { });
app.Map("/", () => Results.Redirect("/api"));
app.MapEndpoints(assembly);
app.MapGraphQL();

app.Run();

public partial class Program { }
