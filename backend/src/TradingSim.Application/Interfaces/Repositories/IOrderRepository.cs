using TradingSim.Domain.Entities;

namespace TradingSim.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task CreateAsync(Order order);
    Task UpdateAsync(Order order);

    Task<Order?> GetByIdAsync(string id);

    Task<List<Order>> GetOpenLimitOrdersAsync();          // for rebuilding books on startup
    Task<List<Order>> GetUserOpenOrdersAsync(string userId);
}