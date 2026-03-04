using TradingSim.Application.DTOs.Orders;
using TradingSim.Application.Interfaces.Repositories;

namespace TradingSim.Application.UseCases.Orders;

public sealed class ListOpenOrdersUseCase
{
    private readonly IOrderRepository _orders;

    public ListOpenOrdersUseCase(IOrderRepository orders)
    {
        _orders = orders;
    }

    public async Task<List<OrderDto>> ExecuteAsync(string userId)
    {
        var list = await _orders.GetUserOpenOrdersAsync(userId);

        return list.Select(o => new OrderDto
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
        }).ToList();
    }
}