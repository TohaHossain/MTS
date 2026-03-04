using TradingSim.Domain.Entities;
using TradingSim.Domain.Enums;

namespace TradingSim.Domain.Matching;

public static class PriceTimePriority
{
    // Best price first, then earlier time first
    public static IOrderedEnumerable<Order> SortBook(IEnumerable<Order> orders, OrderSide side)
    {
        return side == OrderSide.Buy
            ? orders.OrderByDescending(o => o.LimitPrice ?? decimal.MinValue).ThenBy(o => o.CreatedUtc)
            : orders.OrderBy(o => o.LimitPrice ?? decimal.MaxValue).ThenBy(o => o.CreatedUtc);
    }
}