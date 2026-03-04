using TradingSim.Application.DTOs.Orders;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Application.Interfaces.Services;
using TradingSim.Domain.Entities;
using TradingSim.Domain.Enums;

namespace TradingSim.Application.UseCases.Orders;

public sealed class PlaceOrderUseCase
{
    private readonly IInstrumentRepository _instruments;
    private readonly IOrderRepository _orders;
    private readonly ITradeRepository _trades;
    private readonly IMatchingEngine _matching;

    public PlaceOrderUseCase(
        IInstrumentRepository instruments,
        IOrderRepository orders,
        ITradeRepository trades,
        IMatchingEngine matching)
    {
        _instruments = instruments;
        _orders = orders;
        _trades = trades;
        _matching = matching;
    }

    public async Task<OrderDto> ExecuteAsync(string userId, PlaceOrderRequest request)
    {
        // 1) Validate instrument exists
        var instrument = await _instruments.GetBySymbolAsync(request.Symbol)
                         ?? throw new Exception("Instrument not found.");

        // 2) Basic validation
        if (request.Quantity <= 0)
            throw new Exception("Quantity must be positive.");

        if (request.Type == OrderType.Limit && request.LimitPrice is null)
            throw new Exception("LimitPrice is required for LIMIT orders.");

        if (request.Type == OrderType.Market && request.LimitPrice is not null)
            throw new Exception("LimitPrice must be null for MARKET orders.");

        // For demo engine: only Market/Limit are matched (others rejected by engine)
        // But we still allow the request to be saved if you want; here we keep it strict:
        if (request.Type is not (OrderType.Market or OrderType.Limit))
            throw new Exception("For demo: only Market and Limit orders are supported.");

        // 3) Create order
        var now = DateTime.UtcNow;

        var order = new Order
        {
            UserId = userId,
            Symbol = request.Symbol.Trim(),
            Side = request.Side,
            Type = request.Type,
            TimeInForce = request.TimeInForce,
            LimitPrice = request.LimitPrice,
            StopPrice = request.StopPrice,
            Quantity = request.Quantity,
            RemainingQuantity = request.Quantity,
            Status = OrderStatus.New,
            IsActive = true,
            CreatedUtc = now,
            UpdatedUtc = now
        };

        // 4) Persist NEW order first (so it has an Id for trade references)
        await _orders.CreateAsync(order);

        // 5) Match
        var match = _matching.Match(order);

        // 6) Persist resulting trades + orders
        if (match.Trades.Count > 0)
            await _trades.CreateManyAsync(match.Trades);

        // UpdatedOrders includes incoming + any resting orders touched
        foreach (var updated in match.UpdatedOrders)
            await _orders.UpdateAsync(updated);

        // 7) Optionally: update instrument "LastPrice" to last trade price (nice for UI)
        if (match.Trades.Count > 0)
        {
            var lastTradePrice = match.Trades[^1].Price;
            instrument.LastPrice = lastTradePrice;
            instrument.UpdatedUtc = DateTime.UtcNow;
            await _instruments.UpdateAsync(instrument);
        }

        return ToDto(order);
    }

    private static OrderDto ToDto(Order o) => new()
    {
        Id = o.Id,
        Symbol = o.Symbol,
        Side = o.Side,
        Type = o.Type,
        Quantity = o.Quantity,
        RemainingQuantity = o.RemainingQuantity,
        Status = o.Status,
        LimitPrice = o.LimitPrice,
        CreatedUtc = o.CreatedUtc
    };
}