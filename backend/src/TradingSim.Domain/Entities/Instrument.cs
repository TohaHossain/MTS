using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TradingSim.Domain.Entities;

public sealed class Instrument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    // e.g. "DSE:ABC" or "XYZ"
    public string Symbol { get; set; } = default!;
    public string Name { get; set; } = default!;

    // Admin controls this "reference" price for demo
    public decimal LastPrice { get; set; }

    public long MaxQuantity { get; set; }

    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}