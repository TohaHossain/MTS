using TradingSim.Application.DTOs.Power;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Domain.Enums;

namespace TradingSim.Application.UseCases.Power;

public sealed class ReviewPowerUseCase
{
    private readonly IPurchasePowerRequestRepository _repo;
    private readonly IUserRepository _users;

    public ReviewPowerUseCase(IPurchasePowerRequestRepository repo, IUserRepository users)
    {
        _repo = repo;
        _users = users;
    }

    public async Task ExecuteAsync(string requestId, ReviewPowerRequest request)
    {
        var req = await _repo.GetByIdAsync(requestId) ?? throw new Exception("Request not found.");

        if (req.Status != RequestStatus.Pending)
            throw new Exception("Request already reviewed.");

        req.Status = request.Approve ? RequestStatus.Approved : RequestStatus.Declined;
        req.UpdatedUtc = DateTime.UtcNow;

        if (request.Approve)
        {
            var user = await _users.GetByIdAsync(req.UserId);
            if (user != null)
            {
                user.PurchasePower += req.Amount;
                await _users.UpdateAsync(user);
            }
        }

        await _repo.UpdateAsync(req);
    }
}
