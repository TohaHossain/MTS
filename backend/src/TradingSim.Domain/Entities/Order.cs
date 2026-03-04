using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TradingSim.Domain.Enums;

namespace TradingSim.Domain.Entities;

public sealed class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    public string UserId { get; set; } = default!;
    public string Symbol { get; set; } = default!;

    public OrderSide Side { get; set; }
    public OrderType Type { get; set; }
    public TimeInForce TimeInForce { get; set; } = TimeInForce.Day;

    // Optional
    public decimal? LimitPrice { get; set; }
    public decimal? StopPrice { get; set; }

    public long Quantity { get; set; }
    public long RemainingQuantity { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.New;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}