using FluentAssertions;
using TradingSim.Domain.Entities;
using TradingSim.Domain.Enums;
using TradingSim.Domain.Matching;
using TradingSim.Infrastructure.Services;
using Xunit;

namespace TradingSim.Tests.Services;

public class MatchingEngineAdapterTests
{
    [Fact]
    public void LoadOpenLimitOrder_Should_Delegate_To_Engine()
    {
        var engine = new MatchingEngine();
        var adapter = new MatchingEngineAdapter(engine);

        var order = new Order
        {
            Id = "507f1f77bcf86cd799439011",
            Symbol = "GP",
            Side = OrderSide.Buy,
            Type = OrderType.Limit,
            LimitPrice = 100m,
            Quantity = 10,
            RemainingQuantity = 10,
            IsActive = true
        };

        adapter.LoadOpenLimitOrder(order);

        // Verify the order is on the book by placing a matching sell
        var sell = new Order
        {
            Id = "507f1f77bcf86cd799439012",
            Symbol = "GP",
            Side = OrderSide.Sell,
            Type = OrderType.Limit,
            LimitPrice = 100m,
            Quantity = 10,
            RemainingQuantity = 10,
            IsActive = true
        };

        var result = adapter.Match(sell);
        result.Trades.Should().HaveCount(1);
    }
}