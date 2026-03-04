namespace TradingSim.Application.DTOs.Instruments;

public sealed class CreateInstrumentRequest
{
    public string Symbol { get; set; } = default!;
    public string Name { get; set; } = default!;
    public decimal LastPrice { get; set; }
}