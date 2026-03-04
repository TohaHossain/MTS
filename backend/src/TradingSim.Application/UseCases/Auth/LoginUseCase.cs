using TradingSim.Application.DTOs.Auth;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Application.Interfaces.Services;

namespace TradingSim.Application.UseCases.Auth;

public sealed class LoginUseCase
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;

    public LoginUseCase(
        IUserRepository users,
        IPasswordHasher hasher,
        IJwtService jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<AuthResponse> ExecuteAsync(LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email)
                   ?? throw new Exception("Invalid credentials.");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid credentials.");

        return new AuthResponse
        {
            Token = _jwt.GenerateToken(user),
            Email = user.Email,
            Role = user.Role
        };
    }
}