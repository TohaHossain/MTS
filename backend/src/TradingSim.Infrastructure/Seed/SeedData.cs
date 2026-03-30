using MongoDB.Driver;
using TradingSim.Domain.Entities;

namespace TradingSim.Infrastructure.Seed;

public static class SeedData
{
    public static async Task SeedAsync(IMongoDatabase database)
    {
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
            try
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
            catch (Exception ex)
            {
                // 🔥 DO NOT crash app if one item fails
                Console.WriteLine($"Seeding error for {ins.Symbol}: {ex.Message}");
            }
        }

        Console.WriteLine("Seeding completed successfully.");
    }
}