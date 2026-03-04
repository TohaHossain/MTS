using MongoDB.Driver;
using TradingSim.Domain.Entities;
using TradingSim.Infrastructure.Mongo;

namespace TradingSim.Infrastructure.Seed;

public sealed class SeedData
{
    private readonly IMongoCollection<Instrument> _instruments;

    public SeedData(MongoContext ctx)
    {
        _instruments = ctx.Db.GetCollection<Instrument>(Collections.Instruments);
    }

    public async Task EnsureSeededAsync()
    {
        var count = await _instruments.CountDocumentsAsync(_ => true);
        if (count > 0) return;

        var demo = new List<Instrument>
        {
            new() { Symbol="AAA", Name="Alpha A", LastPrice=100m },
            new() { Symbol="AAB", Name="Alpha B", LastPrice=105m },
            new() { Symbol="AAC", Name="Alpha C", LastPrice=98m },
            new() { Symbol="BAA", Name="Beta A",  LastPrice=210m },
            new() { Symbol="BAB", Name="Beta B",  LastPrice=215m },
            new() { Symbol="BAC", Name="Beta C",  LastPrice=205m },
            new() { Symbol="CAA", Name="Gamma A", LastPrice=55m },
            new() { Symbol="CAB", Name="Gamma B", LastPrice=57m },
            new() { Symbol="CAC", Name="Gamma C", LastPrice=53m },
            new() { Symbol="DAA", Name="Delta A", LastPrice=320m },
            new() { Symbol="DAB", Name="Delta B", LastPrice=310m },
            new() { Symbol="DAC", Name="Delta C", LastPrice=315m },
            new() { Symbol="EAA", Name="Epsilon A", LastPrice=12.5m },
            new() { Symbol="EAB", Name="Epsilon B", LastPrice=13.0m },
            new() { Symbol="EAC", Name="Epsilon C", LastPrice=12.2m },
            new() { Symbol="FAA", Name="Zeta A", LastPrice=78m },
            new() { Symbol="FAB", Name="Zeta B", LastPrice=79m },
            new() { Symbol="FAC", Name="Zeta C", LastPrice=77m },
            new() { Symbol="GAA", Name="Eta A", LastPrice=460m },
            new() { Symbol="GAB", Name="Eta B", LastPrice=455m }
        };

        await _instruments.InsertManyAsync(demo);
    }
}