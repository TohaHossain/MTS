using TradingSim.Domain.Entities;
using TradingSim.Domain.Enums;

namespace TradingSim.Domain.Matching;

public sealed class OrderBook
{
    // Limit orders only (for demo)
    private readonly List<Order> _bids = new();
    private readonly List<Order> _asks = new();

    public IReadOnlyList<Order> Bids => _bids;
    public IReadOnlyList<Order> Asks => _asks;

    public void AddLimit(Order order)
    {
        if (order.Type != OrderType.Limit)
            throw new InvalidOperationException("OrderBook accepts only LIMIT orders.");

        if (order.Side == OrderSide.Buy) _bids.Add(order);
        else _asks.Add(order);
    }

    public void Remove(string orderId)
    {
        _bids.RemoveAll(o => o.Id == orderId);
        _asks.RemoveAll(o => o.Id == orderId);
    }

    public Order? GetBestOpposing(Order incoming)
    {
        if (incoming.Side == OrderSide.Buy)
        {
            var sorted = PriceTimePriority.SortBook(_asks.Where(IsActiveLimit), OrderSide.Sell);
            return sorted.FirstOrDefault();
        }

        var sortedBids = PriceTimePriority.SortBook(_bids.Where(IsActiveLimit), OrderSide.Buy);
        return sortedBids.FirstOrDefault();
    }

    public IEnumerable<Order> GetSortedSameSide(OrderSide side)
        => PriceTimePriority.SortBook(
            (side == OrderSide.Buy ? _bids : _asks).Where(IsActiveLimit),
            side);

    private static bool IsActiveLimit(Order o)
        => o.IsActive && o.Status is OrderStatus.New or OrderStatus.PartiallyFilled && o.RemainingQuantity > 0;
}