using TradingSim.Application.Interfaces.Services;
using TradingSim.Domain.Entities;
using TradingSim.Domain.Matching;

namespace TradingSim.Infrastructure.Services;

public sealed class MatchingEngineAdapter : IMatchingEngine
{
    private readonly MatchingEngine _engine;

    public MatchingEngineAdapter(MatchingEngine engine)
    {
        _engine = engine;
    }

    public void LoadOpenLimitOrder(Order order) => _engine.LoadOpenLimitOrder(order);

    public MatchResult Match(Order order) => _engine.Match(order);

    public void RemoveFromBook(string symbol, string orderId) => _engine.RemoveFromBook(symbol, orderId);
}