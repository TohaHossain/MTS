using TradingSim.Domain.Entities;

namespace TradingSim.Application.Interfaces.Repositories;

public interface ITradeRepository
{
    Task CreateManyAsync(IEnumerable<Trade> trades);
    Task<List<Trade>> GetAllAsync();
    Task<List<Trade>> GetByUserAsync(string userId);
}