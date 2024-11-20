using Application.BackgroundServices;
using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureIoC(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ILogger, Logger>();
        services.AddSingleton<ICurrencyCache, CurrencyCache>();
        services.AddSingleton<IMarketDataService, BinanceService>();
        services.AddHostedService<CacheUpdaterService>();

        return services;
    }
}