using MongoDB.Driver;
using MongoDB.Bson;
using TradingSim.Domain.Entities;

namespace TradingSim.Infrastructure.Seed;

public static class SeedData
{
    public static async Task SeedAsync(IMongoDatabase database)
    {
        // 🔥 Wait for Mongo PRIMARY before doing anything
        await WaitForPrimaryAsync(database);

        var collection = database.GetCollection<Instrument>("instruments");

        var instruments = new List<Instrument>
        {
            new() { Symbol = "GP", Name = "Grameenphone Ltd." },
            new() { Symbol = "BATBC", Name = "British American Tobacco Bangladesh" },
            new() { Symbol = "BEXIMCO", Name = "Beximco Ltd." },
            new() { Symbol = "SQURPHARMA", Name = "Square Pharmaceuticals Ltd." },
            new() { Symbol = "RENATA", Name = "Renata Ltd." },
            new() { Symbol = "BRACBANK", Name = "BRAC Bank Ltd." },
            new() { Symbol = "CITYBANK", Name = "The City Bank Ltd." },
            new() { Symbol = "DUTCHBANGL", Name = "Dutch-Bangla Bank Ltd." },
            new() { Symbol = "IFIC", Name = "IFIC Bank Ltd." },
            new() { Symbol = "PUBALIBANK", Name = "Pubali Bank Ltd." },
            new() { Symbol = "OLYMPIC", Name = "Olympic Industries Ltd." },
            new() { Symbol = "ACI", Name = "Advanced Chemical Industries Ltd." },
            new() { Symbol = "ACIFORMULA", Name = "ACI Formulations Ltd." },
            new() { Symbol = "ACMELAB", Name = "ACME Laboratories Ltd." },
            new() { Symbol = "IBNSINA", Name = "Ibn Sina Pharmaceutical Ltd." },
            new() { Symbol = "MARICO", Name = "Marico Bangladesh Ltd." },
            new() { Symbol = "POWERGRID", Name = "Power Grid Company of Bangladesh Ltd." },
            new() { Symbol = "SUMITPOWER", Name = "Summit Power Ltd." },
            new() { Symbol = "BSRMLTD", Name = "BSRM Ltd." },
            new() { Symbol = "WALTONHIL", Name = "Walton Hi-Tech Industries Ltd." }
        };

        var rnd = new Random();

        foreach (var ins in instruments)
        {
            var filter = Builders<Instrument>.Filter.Eq(x => x.Symbol, ins.Symbol);

            var existing = await collection.Find(filter).FirstOrDefaultAsync();

            var qty = existing != null && existing.MaxQuantity > 0
                ? existing.MaxQuantity
                : rnd.Next(1000, 10000);

            var price = existing != null && existing.LastPrice > 0
                ? existing.LastPrice
                : Math.Round((decimal)(rnd.NextDouble() * 450 + 50), 2);

            var update = Builders<Instrument>.Update
                .Set(x => x.Symbol, ins.Symbol)
                .Set(x => x.Name, ins.Name)
                .Set(x => x.LastPrice, price)
                .Set(x => x.MaxQuantity, qty)
                .Set(x => x.UpdatedUtc, DateTime.UtcNow);

            await collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }
    }

    // 🔥 CRITICAL PART
    private static async Task WaitForPrimaryAsync(IMongoDatabase database)
    {
        var client = database.Client;

        for (int i = 0; i < 10; i++)
        {
            try
            {
                var adminDb = client.GetDatabase("admin");

                var result = await adminDb.RunCommandAsync<BsonDocument>(
                    new BsonDocument("isMaster", 1)
                );

                if (result.Contains("ismaster") && result["ismaster"].AsBoolean)
                {
                    Console.WriteLine("Mongo PRIMARY is ready.");
                    return;
                }
            }
            catch
            {
                // ignore errors
            }

            Console.WriteLine("Waiting for Mongo PRIMARY...");
            await Task.Delay(3000);
        }

        throw new Exception("Mongo PRIMARY not available after retries.");
    }
}