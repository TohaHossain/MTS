using TradingSim.Application.Interfaces.Repositories;

namespace TradingSim.Application.UseCases.Power;

public sealed class GetBalanceUseCase
{
    private readonly IUserRepository _users;

    public GetBalanceUseCase(IUserRepository users)
    {
        _users = users;
    }

    public async Task<decimal> ExecuteAsync(string userId)
    {
        var user = await _users.GetByIdAsync(userId) ?? throw new Exception("User not found.");
        return user.PurchasePower;
    }
}
