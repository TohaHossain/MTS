using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Application.Interfaces.Services;
using TradingSim.Domain.Enums;

namespace TradingSim.Application.UseCases.Orders;

public sealed class CancelOrderUseCase
{
    private readonly IOrderRepository _orders;
    private readonly IMatchingEngine _engine;
    private readonly IUserRepository _users;

    public CancelOrderUseCase(IOrderRepository orders, IMatchingEngine engine, IUserRepository users)
    {
        _orders = orders;
        _engine = engine;
        _users = users;
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

        if (order.Side == OrderSide.Buy && order.RemainingQuantity > 0)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user != null)
            {
                // In demo fallback, Market orders realistically shouldn't be cancelable, but safe fallback to limit price.
                var price = order.LimitPrice ?? 0;
                user.PurchasePower += price * order.RemainingQuantity;
                await _users.UpdateAsync(user);
            }
        }

        await _orders.UpdateAsync(order);

        // Remove from in-memory book
        if (order.Type == OrderType.Limit)
            _engine.RemoveFromBook(order.Symbol, order.Id);
    }
}