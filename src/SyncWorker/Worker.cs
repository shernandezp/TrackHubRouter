using MediatR;
using TrackHubRouter.Application.Positions.Commands.Sync;

namespace TrackHubRouter.SyncWorker;

public class Worker(ILogger<Worker> logger, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await SyncData(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing data.");
            }

            await Task.Delay(10000, stoppingToken);
        }
    }

    private async Task SyncData(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        try
        {
            await sender.Send(new SyncPositionCommand(), stoppingToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error syncing data. {ex.Message}");
        }
    }

}
