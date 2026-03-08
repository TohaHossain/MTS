using TradingSim.Application.DTOs.Power;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Domain.Entities;

namespace TradingSim.Application.UseCases.Power;

public sealed class RequestPowerUseCase
{
    private readonly IPurchasePowerRequestRepository _repo;
    private readonly IUserRepository _users;

    public RequestPowerUseCase(IPurchasePowerRequestRepository repo, IUserRepository users)
    {
        _repo = repo;
        _users = users;
    }

    public async Task ExecuteAsync(string userId, RequestPowerRequest request)
    {
        if (request.Amount < 1000 || request.Amount > 10000000)
            throw new Exception("Amount must be between 1,000 and 10,000,000.");

        var user = await _users.GetByIdAsync(userId) ?? throw new Exception("User not found.");

        var req = new PurchasePowerRequest
        {
            UserId = user.Id,
            UserEmail = user.Email,
            Amount = request.Amount
        };

        await _repo.CreateAsync(req);
    }
}
