using TradingSim.Domain.Entities;
using TradingSim.Domain.Enums;

namespace TradingSim.Domain.Matching;

public sealed class MatchingEngine
{
    private readonly Dictionary<string, OrderBook> _books = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lock = new();

    public void LoadOpenLimitOrder(Order order)
    {
        if (order.Type != OrderType.Limit) return;
        if (!order.IsActive) return;
        if (order.RemainingQuantity <= 0) return;

        lock (_lock)
        {
            var book = GetOrCreate(order.Symbol);
            book.AddLimit(order);
        }
    }

    public void RemoveFromBook(string symbol, string orderId)
    {
        lock (_lock)
        {
            if (_books.TryGetValue(symbol, out var book))
                book.Remove(orderId);
        }
    }

    public MatchResult Match(Order incoming)
    {
        if (incoming.Type is not (OrderType.Market or OrderType.Limit))
        {
            incoming.Status = OrderStatus.Rejected;
            incoming.IsActive = false;
            incoming.UpdatedUtc = DateTime.UtcNow;
            return new MatchResult { UpdatedOrders = { incoming } };
        }

        lock (_lock)
        {
            var book = GetOrCreate(incoming.Symbol);
            var result = new MatchResult();

            while (incoming.RemainingQuantity > 0)
            {
                var best = book.GetBestOpposing(incoming);
                if (best is null) break;

                if (incoming.Type == OrderType.Limit && !IsPriceMatch(incoming, best))
                    break;

                var tradePrice = best.LimitPrice!.Value;
                var fillQty = Math.Min(incoming.RemainingQuantity, best.RemainingQuantity);

                incoming.RemainingQuantity -= fillQty;
                best.RemainingQuantity -= fillQty;

                incoming.Status = incoming.RemainingQuantity == 0 ? OrderStatus.Filled : OrderStatus.PartiallyFilled;
                best.Status = best.RemainingQuantity == 0 ? OrderStatus.Filled : OrderStatus.PartiallyFilled;

                incoming.UpdatedUtc = DateTime.UtcNow;
                best.UpdatedUtc = DateTime.UtcNow;

                if (best.RemainingQuantity == 0)
                {
                    best.IsActive = false;
                    book.Remove(best.Id);
                }

                result.Trades.Add(CreateTrade(incoming, best, tradePrice, fillQty));
                result.UpdatedOrders.Add(best);
            }

            if (incoming.RemainingQuantity == 0)
            {
                incoming.IsActive = false;
            }
            else
            {
                if (incoming.Type == OrderType.Market)
                {
                    incoming.IsActive = false;
                    incoming.Status = result.Trades.Count > 0 ? incoming.Status : OrderStatus.Cancelled;
                }
                else
                {
                    if (incoming.TimeInForce is TimeInForce.Day or TimeInForce.GTC)
                        book.AddLimit(incoming);
                    else
                    {
                        incoming.IsActive = false;
                        incoming.Status = result.Trades.Count > 0 ? incoming.Status : OrderStatus.Cancelled;
                    }
                }
            }

            result.UpdatedOrders.Add(incoming);
            return result;
        }
    }

    private OrderBook GetOrCreate(string symbol)
    {
        if (!_books.TryGetValue(symbol, out var book))
        {
            book = new OrderBook();
            _books[symbol] = book;
        }
        return book;
    }

    private static bool IsPriceMatch(Order incoming, Order resting)
    {
        if (incoming.Side == OrderSide.Buy)
            return incoming.LimitPrice!.Value >= resting.LimitPrice!.Value;

        return incoming.LimitPrice!.Value <= resting.LimitPrice!.Value;
    }

    private static Trade CreateTrade(Order incoming, Order resting, decimal price, long qty)
    {
        Order buy = incoming.Side == OrderSide.Buy ? incoming : resting;
        Order sell = incoming.Side == OrderSide.Sell ? incoming : resting;

        return new Trade
        {
            Symbol = incoming.Symbol,
            Price = price,
            Quantity = qty,
            BuyOrderId = buy.Id,
            SellOrderId = sell.Id,
            BuyerUserId = buy.UserId,
            SellerUserId = sell.UserId,
            ExecutedUtc = DateTime.UtcNow
        };
    }
}