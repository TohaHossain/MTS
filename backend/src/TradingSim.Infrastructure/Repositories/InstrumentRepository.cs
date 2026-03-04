using MongoDB.Driver;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Domain.Entities;
using TradingSim.Infrastructure.Mongo;

namespace TradingSim.Infrastructure.Repositories;

public sealed class InstrumentRepository : IInstrumentRepository
{
    private readonly IMongoCollection<Instrument> _col;

    public InstrumentRepository(MongoContext ctx)
    {
        _col = ctx.Db.GetCollection<Instrument>(Collections.Instruments);

        // Ensure unique Symbol (demo-friendly)
        var indexKeys = Builders<Instrument>.IndexKeys.Ascending(x => x.Symbol);
        var indexModel = new CreateIndexModel<Instrument>(indexKeys, new CreateIndexOptions { Unique = true });
        _col.Indexes.CreateOne(indexModel);
    }

    public async Task<List<Instrument>> GetAllAsync()
        => await _col.Find(_ => true).SortBy(x => x.Symbol).ToListAsync();

    public async Task<Instrument?> GetBySymbolAsync(string symbol)
        => await _col.Find(x => x.Symbol == symbol).FirstOrDefaultAsync();

    public async Task CreateAsync(Instrument instrument)
        => await _col.InsertOneAsync(instrument);

    public async Task UpdateAsync(Instrument instrument)
        => await _col.ReplaceOneAsync(x => x.Id == instrument.Id, instrument);
}