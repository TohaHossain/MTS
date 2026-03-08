using TradingSim.Domain.Entities;

namespace TradingSim.Application.Interfaces.Repositories;

public interface IPurchasePowerRequestRepository
{
    Task<List<PurchasePowerRequest>> GetByUserIdAsync(string userId);
    Task<List<PurchasePowerRequest>> GetPendingAsync();
    Task<PurchasePowerRequest?> GetByIdAsync(string id);
    Task CreateAsync(PurchasePowerRequest request);
    Task UpdateAsync(PurchasePowerRequest request);
}
