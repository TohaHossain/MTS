using TradingSim.Application.DTOs.Trades;
using TradingSim.Application.Interfaces.Repositories;

namespace TradingSim.Application.UseCases.Trades;

public sealed class GetMyTradesUseCase
{
    private readonly ITradeRepository _trades;

    public GetMyTradesUseCase(ITradeRepository trades)
    {
        _trades = trades;
    }

    public async Task<List<TradeDto>> ExecuteAsync(string userId)
    {
        var list = await _trades.GetByUserAsync(userId);

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