using TradingSim.Application.DTOs.Power;
using TradingSim.Application.Interfaces.Repositories;

namespace TradingSim.Application.UseCases.Power;

public sealed class ListPendingPowerRequestsUseCase
{
    private readonly IPurchasePowerRequestRepository _repo;

    public ListPendingPowerRequestsUseCase(IPurchasePowerRequestRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<PowerRequestDto>> ExecuteAsync()
    {
        var list = await _repo.GetPendingAsync();
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
