using System.Reflection;
using TrackHubRouter.Infrastructure;
using TrackHubRouter.Web.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();

// Add HealthChecks
builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/api"));

var assembly = Assembly.GetExecutingAssembly();

app.MapEndpoints(assembly);

app.MapGraphQL();

app.Run();

public partial class Program { }
