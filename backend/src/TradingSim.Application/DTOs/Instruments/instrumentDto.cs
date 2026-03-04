namespace TradingSim.Application.DTOs.Instruments;

public sealed class InstrumentDto
{
    public string Id { get; set; } = default!;
    public string Symbol { get; set; } = default!;
    public string Name { get; set; } = default!;
    public decimal LastPrice { get; set; }
}