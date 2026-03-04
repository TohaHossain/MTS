using MongoDB.Driver;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Domain.Entities;
using TradingSim.Domain.Enums;
using TradingSim.Infrastructure.Mongo;

namespace TradingSim.Infrastructure.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _col;

    public OrderRepository(MongoContext ctx)
    {
        _col = ctx.Db.GetCollection<Order>(Collections.Orders);

        // Helpful indexes
        _col.Indexes.CreateOne(new CreateIndexModel<Order>(
            Builders<Order>.IndexKeys.Ascending(x => x.UserId).Ascending(x => x.CreatedUtc)));

        _col.Indexes.CreateOne(new CreateIndexModel<Order>(
            Builders<Order>.IndexKeys.Ascending(x => x.Symbol).Ascending(x => x.CreatedUtc)));
    }

    public async Task CreateAsync(Order order)
        => await _col.InsertOneAsync(order);

    public async Task UpdateAsync(Order order)
        => await _col.ReplaceOneAsync(x => x.Id == order.Id, order);

    public async Task<Order?> GetByIdAsync(string id)
        => await _col.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<Order>> GetOpenLimitOrdersAsync()
    {
        // used for rebuilding books on startup
        return await _col.Find(x =>
                x.IsActive &&
                (x.Status == OrderStatus.New || x.Status == OrderStatus.PartiallyFilled) &&
                x.Type == OrderType.Limit &&
                x.RemainingQuantity > 0
            )
            .ToListAsync();
    }

    public async Task<List<Order>> GetUserOpenOrdersAsync(string userId)
    {
        return await _col.Find(x =>
                x.UserId == userId &&
                x.IsActive &&
                (x.Status == OrderStatus.New || x.Status == OrderStatus.PartiallyFilled) &&
                x.RemainingQuantity > 0
            )
            .SortByDescending(x => x.CreatedUtc)
            .ToListAsync();
    }
}