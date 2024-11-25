using System.Reflection;
using Ardalis.GuardClauses;
using Common.Application;
using TrackHubRouter.Web.GraphQL;

var builder = WebApplication.CreateBuilder(args);

var allowedCORSOrigins = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string>();
Guard.Against.Null(allowedCORSOrigins, message: $"Allowed Origins configuration for CORS not loaded");

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddAppManagerContext();
builder.Services.AddCommonContext(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();

builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>();

builder.Services.AddCors(options => options
    .AddPolicy("AllowFrontend",
        builder => builder
                    .WithOrigins(allowedCORSOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));

// Configure HSTS
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365 * 2);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

var app = builder.Build();

app.UseHeaderPropagation();

// Enable CORS
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseExceptionHandler(options => { });
app.MapEndpoints(Assembly.GetExecutingAssembly());

app.MapGraphQL();

app.Run();
