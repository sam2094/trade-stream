using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using System;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application.BackgroundServices;

public class CacheUpdaterService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _services;
    private readonly IMarketDataService _marketDataService;

    public CacheUpdaterService(
        IServiceProvider services,
        ILogger logger,
        IMarketDataService marketDataService)
    {
        _services = services;
        _logger = logger;
        _marketDataService = marketDataService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _logger.LogToConsoleAsync("CacheUpdaterService is starting.");

        try
        {
            await _marketDataService.StartAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                await _logger.LogToConsoleAsync("Updating cache...");

                using (var scope = _services.CreateScope())
                {
                    var currencyCache = scope.ServiceProvider.GetRequiredService<ICurrencyCache>();
                    var result = currencyCache.GetSnapshot();
                    var result2 = "";
                }

                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            await _logger.LogToConsoleAsync($"Error in CacheUpdaterService \n {ex.Message}");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await _logger.LogToConsoleAsync("CacheUpdaterService is stopping.");

        await _marketDataService.StopAsync();

        await base.StopAsync(stoppingToken);
    }

    public override void Dispose()
    {
        _marketDataService.Dispose();
        base.Dispose();
    }
}