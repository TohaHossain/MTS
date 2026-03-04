using MongoDB.Driver;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Domain.Entities;
using TradingSim.Infrastructure.Mongo;

namespace TradingSim.Infrastructure.Repositories;

public sealed class TradeRepository : ITradeRepository
{
    private readonly IMongoCollection<Trade> _col;

    public TradeRepository(MongoContext ctx)
    {
        _col = ctx.Db.GetCollection<Trade>(Collections.Trades);

        _col.Indexes.CreateOne(new CreateIndexModel<Trade>(
            Builders<Trade>.IndexKeys.Ascending(x => x.Symbol).Descending(x => x.ExecutedUtc)));
    }

    public async Task CreateManyAsync(IEnumerable<Trade> trades)
    {
        var list = trades.ToList();
        if (list.Count == 0) return;

        await _col.InsertManyAsync(list);
    }

    public async Task<List<Trade>> GetAllAsync()
        => await _col.Find(_ => true).SortByDescending(x => x.ExecutedUtc).ToListAsync();

    public async Task<List<Trade>> GetByUserAsync(string userId)
    {
        // user can be buyer or seller
        return await _col.Find(x => x.BuyerUserId == userId || x.SellerUserId == userId)
            .SortByDescending(x => x.ExecutedUtc)
            .ToListAsync();
    }
}