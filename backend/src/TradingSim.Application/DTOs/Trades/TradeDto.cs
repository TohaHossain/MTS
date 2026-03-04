namespace TradingSim.Application.DTOs.Trades;

public sealed class TradeDto
{
    public string Id { get; set; } = default!;
    public string Symbol { get; set; } = default!;
    public decimal Price { get; set; }
    public long Quantity { get; set; }
    public DateTime ExecutedUtc { get; set; }
}