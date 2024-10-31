using Common.Application;
using TrackHub.Router.Infrastructure.Common;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.SyncWorker;

var builder = Host.CreateApplicationBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddAppManagerContext(false);
builder.Services.AddCommonContext(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSingleton<IExecutionIntervalManager, ExecutionIntervalManager>();
builder.Services.AddWorkerServices();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
