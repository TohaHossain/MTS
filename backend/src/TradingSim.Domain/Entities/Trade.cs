using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TradingSim.Domain.Entities;

public sealed class Trade
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    public string Symbol { get; set; } = default!;

    public decimal Price { get; set; }
    public long Quantity { get; set; }

    public string BuyOrderId { get; set; } = default!;
    public string SellOrderId { get; set; } = default!;

    public string BuyerUserId { get; set; } = default!;
    public string SellerUserId { get; set; } = default!;

    public DateTime ExecutedUtc { get; set; } = DateTime.UtcNow;
}