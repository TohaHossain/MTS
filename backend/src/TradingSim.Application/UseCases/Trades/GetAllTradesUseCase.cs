using TradingSim.Application.DTOs.Trades;
using TradingSim.Application.Interfaces.Repositories;

namespace TradingSim.Application.UseCases.Trades;

public sealed class GetAllTradesUseCase
{
    private readonly ITradeRepository _trades;

    public GetAllTradesUseCase(ITradeRepository trades)
    {
        _trades = trades;
    }

    public async Task<List<TradeDto>> ExecuteAsync()
    {
        var list = await _trades.GetAllAsync();

        return list
            .OrderByDescending(t => t.ExecutedUtc)
            .Select(t => new TradeDto
            {
                Id = t.Id,
                Symbol = t.Symbol,
                Price = t.Price,
                Quantity = t.Quantity,
                ExecutedUtc = t.ExecutedUtc
            })
            .ToList();
    }
}