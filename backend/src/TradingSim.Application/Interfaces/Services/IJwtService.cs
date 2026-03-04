using TradingSim.Domain.Entities;

namespace TradingSim.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}