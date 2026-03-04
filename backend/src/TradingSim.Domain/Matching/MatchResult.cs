using TradingSim.Domain.Entities;

namespace TradingSim.Domain.Matching;

public sealed class MatchResult
{
    public List<Trade> Trades { get; } = new();
    public List<Order> UpdatedOrders { get; } = new(); // includes incoming + resting updated snapshots
}