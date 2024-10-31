using System.Reflection;
using Common.Application;
using TrackHubRouter.Web.GraphQL;

var builder = WebApplication.CreateBuilder(args);

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
                    .WithOrigins("https://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));

var app = builder.Build();

app.UseHeaderPropagation();

// Enable CORS
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseExceptionHandler(options => { });
app.MapEndpoints(Assembly.GetExecutingAssembly());

app.MapGraphQL();

app.Run();
