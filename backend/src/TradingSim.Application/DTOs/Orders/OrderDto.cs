using TradingSim.Domain.Enums;

namespace TradingSim.Application.DTOs.Orders;

public sealed class OrderDto
{
    public string Id { get; set; } = default!;
    public string Symbol { get; set; } = default!;
    public OrderSide Side { get; set; }
    public OrderType Type { get; set; }
    public long Quantity { get; set; }
    public long RemainingQuantity { get; set; }
    public OrderStatus Status { get; set; }
    public decimal? LimitPrice { get; set; }
    public DateTime CreatedUtc { get; set; }
}