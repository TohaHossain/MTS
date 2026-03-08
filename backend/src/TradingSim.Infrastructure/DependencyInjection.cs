using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Application.Interfaces.Services;
using TradingSim.Domain.Matching;
using TradingSim.Infrastructure.Mongo;
using TradingSim.Infrastructure.Repositories;
using TradingSim.Infrastructure.Security;
using TradingSim.Infrastructure.Seed;
using TradingSim.Infrastructure.Services;

namespace TradingSim.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<MongoOptions>(cfg.GetSection("Mongo"));
        services.Configure<AdminSeedOptions>(cfg.GetSection("Seed:Admin"));

        services.AddSingleton<MongoContext>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IInstrumentRepository, InstrumentRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITradeRepository, TradeRepository>();
        services.AddScoped<IPurchasePowerRequestRepository, PurchasePowerRequestRepository>();

        // Security
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtService, JwtService>();

        // Matching Engine (singleton in-memory)
        services.AddSingleton<MatchingEngine>();
        services.AddSingleton<IMatchingEngine, MatchingEngineAdapter>();

        // Seed + startup load
        //services.AddSingleton<SeedData>();
        services.AddHostedService<StartupLoader>();

        return services;
    }
}