using TradingSim.Domain.Entities;
using TradingSim.Domain.Matching;

namespace TradingSim.Application.Interfaces.Services;

public interface IMatchingEngine
{
    void LoadOpenLimitOrder(Order order);
    MatchResult Match(Order order);

    // remove resting limit order from book (used on cancel)
    void RemoveFromBook(string symbol, string orderId);
}