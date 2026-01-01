// Copyright (c) 2025 Sergio Hernandez. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using Common.Mediator;
using TrackHubRouter.Application.DevicePositions.Commands.Sync;

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
