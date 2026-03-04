using TradingSim.Domain.Enums;

namespace TradingSim.Application.DTOs.Orders;

public sealed class PlaceOrderRequest
{
    public string Symbol { get; set; } = default!;
    public OrderSide Side { get; set; }
    public OrderType Type { get; set; }
    public TimeInForce TimeInForce { get; set; }

    public decimal? LimitPrice { get; set; }
    public decimal? StopPrice { get; set; }

    public long Quantity { get; set; }
}