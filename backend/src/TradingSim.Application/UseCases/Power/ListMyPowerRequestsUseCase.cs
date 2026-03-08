using TradingSim.Application.DTOs.Power;
using TradingSim.Application.Interfaces.Repositories;

namespace TradingSim.Application.UseCases.Power;

public sealed class ListMyPowerRequestsUseCase
{
    private readonly IPurchasePowerRequestRepository _repo;

    public ListMyPowerRequestsUseCase(IPurchasePowerRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<PowerRequestDto>> ExecuteAsync(string userId)
    {
        var list = await _repo.GetByUserIdAsync(userId);
        return list.Select(x => new PowerRequestDto
        {
            Id = x.Id,
            UserId = x.UserId,
            UserEmail = x.UserEmail,
            Amount = x.Amount,
            Status = x.Status,
            CreatedUtc = x.CreatedUtc
        }).ToList();
    }
}
