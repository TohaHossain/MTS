using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Application.Interfaces.Services;
using TradingSim.Domain.Entities;
using TradingSim.Domain.Enums;

namespace TradingSim.Infrastructure.Seed;

public sealed class StartupLoader : IHostedService
{
    private readonly SeedData _seed;
    private readonly IOrderRepository _orders;
    private readonly IMatchingEngine _engine;
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly AdminSeedOptions _admin;

    public StartupLoader(
        SeedData seed,
        IOrderRepository orders,
        IMatchingEngine engine,
        IUserRepository users,
        IPasswordHasher hasher,
        IOptions<AdminSeedOptions> adminOptions)
    {
        _seed = seed;
        _orders = orders;
        _engine = engine;
        _users = users;
        _hasher = hasher;
        _admin = adminOptions.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 1) Seed instruments
        await _seed.EnsureSeededAsync();

        // 2) Seed admin user if missing
        var adminUser = await _users.GetByEmailAsync(_admin.Email);
        if (adminUser is null)
        {
            await _users.CreateAsync(new User
            {
                Email = _admin.Email,
                PasswordHash = _hasher.Hash(_admin.Password),
                Role = UserRole.Admin,
                CreatedUtc = DateTime.UtcNow
            });
        }

        // 3) Rebuild in-memory books with open LIMIT orders
        var open = await _orders.GetOpenLimitOrdersAsync();
        foreach (var o in open)
        {
            if (o.Type == OrderType.Limit)
                _engine.LoadOpenLimitOrder(o);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}