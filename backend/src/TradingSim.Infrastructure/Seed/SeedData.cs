using MongoDB.Driver;
using TradingSim.Domain.Entities;

public static class SeedData
{
    public static async Task SeedAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<Instrument>("Instruments");

        var count = await collection.CountDocumentsAsync(_ => true);
        if (count > 0) return;

        var instruments = new List<Instrument>
        {
            new Instrument { Symbol = "AAPL", Name = "Apple Inc." },
            new Instrument { Symbol = "MSFT", Name = "Microsoft Corp." },
            new Instrument { Symbol = "BTCUSD", Name = "Bitcoin / USD" }
        };

        await collection.InsertManyAsync(instruments);
    }
}