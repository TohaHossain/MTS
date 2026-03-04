using TradingSim.Domain.Enums;

namespace TradingSim.Application.DTOs.Auth;

public sealed class AuthResponse
{
    public string Token { get; set; } = default!;
    public string Email { get; set; } = default!;
    public UserRole Role { get; set; }
}