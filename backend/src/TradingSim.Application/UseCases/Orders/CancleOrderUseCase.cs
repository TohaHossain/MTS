using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Application.Interfaces.Services;
using TradingSim.Domain.Enums;

namespace TradingSim.Application.UseCases.Orders;

public sealed class CancelOrderUseCase
{
    private readonly IOrderRepository _orders;
    private readonly IMatchingEngine _engine;

    public CancelOrderUseCase(IOrderRepository orders, IMatchingEngine engine)
    {
        _orders = orders;
        _engine = engine;
    }

    public async Task ExecuteAsync(string userId, string orderId)
    {
        var order = await _orders.GetByIdAsync(orderId)
                    ?? throw new Exception("Order not found.");

        if (!order.IsActive || order.Status is OrderStatus.Filled or OrderStatus.Cancelled)
            throw new Exception("Order is not cancellable.");

        if (order.UserId != userId)
            throw new Exception("You can only cancel your own orders.");

        // Update DB state
        order.IsActive = false;
        order.Status = OrderStatus.Cancelled;
        order.UpdatedUtc = DateTime.UtcNow;

        await _orders.UpdateAsync(order);

        // Remove from in-memory book
        if (order.Type == OrderType.Limit)
            _engine.RemoveFromBook(order.Symbol, order.Id);
    }
}