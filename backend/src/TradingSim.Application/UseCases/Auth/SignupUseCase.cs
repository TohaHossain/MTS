using TradingSim.Application.DTOs.Auth;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Application.Interfaces.Services;
using TradingSim.Domain.Entities;
using TradingSim.Domain.Enums;

namespace TradingSim.Application.UseCases.Auth;

public sealed class SignupUseCase
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public SignupUseCase(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task ExecuteAsync(SignupRequest request)
    {
        var existing = await _users.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new Exception("User already exists.");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = _hasher.Hash(request.Password),
            Role = UserRole.Client
        };

        await _users.CreateAsync(user);
    }
}