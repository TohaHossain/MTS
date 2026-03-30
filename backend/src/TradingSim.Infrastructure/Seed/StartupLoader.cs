using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<StartupLoader> _logger;

    public StartupLoader(IServiceScopeFactory scopeFactory, ILogger<StartupLoader> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        const int maxRetries = 10;
        int attempt = 0;

        while (attempt < maxRetries)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var mongo = scope.ServiceProvider.GetRequiredService<MongoContext>();

                _logger.LogInformation("Seeding attempt {Attempt}", attempt + 1);

                // 🔥 Seed instruments (now includes Mongo readiness check)
                await SeedData.SeedAsync(mongo.Db);

                _logger.LogInformation("Instrument seeding completed.");

                var opts = scope.ServiceProvider.GetRequiredService<IOptions<AdminSeedOptions>>().Value;

                if (string.IsNullOrWhiteSpace(opts.Email) || string.IsNullOrWhiteSpace(opts.Password))
                {
                    _logger.LogWarning("Admin seed skipped: Email/Password not configured.");
                    return;
                }

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

                    _logger.LogInformation("Admin user created: {Email}", opts.Email);
                }
                else
                {
                    _logger.LogInformation("Admin user already exists: {Email}", opts.Email);
                }

                return; // ✅ success → exit loop
            }
            catch (Exception ex)
            {
                attempt++;

                _logger.LogError(ex, "Seeding failed (attempt {Attempt})", attempt);

                if (attempt >= maxRetries)
                {
                    _logger.LogCritical("Seeding failed after max retries.");
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}