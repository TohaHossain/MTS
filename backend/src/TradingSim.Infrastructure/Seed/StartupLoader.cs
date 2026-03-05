using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Application.Interfaces.Services;
using TradingSim.Domain.Entities;
using TradingSim.Domain.Enums;
using TradingSim.Infrastructure.Mongo;

namespace TradingSim.Infrastructure.Seed;

public sealed class StartupLoader : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public StartupLoader(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        // Seed instruments
        var mongo = scope.ServiceProvider.GetRequiredService<MongoContext>();
        await SeedData.SeedAsync(mongo.Db);

        // Seed admin user
        var opts = scope.ServiceProvider.GetRequiredService<IOptions<AdminSeedOptions>>().Value;
        var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var existing = await users.GetByEmailAsync(opts.Email);
        if (existing is null)
        {
            await users.CreateAsync(new User
            {
                Email = opts.Email,
                PasswordHash = hasher.Hash(opts.Password),
                Role = UserRole.Admin
            });
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}