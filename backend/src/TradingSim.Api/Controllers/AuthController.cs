using Microsoft.AspNetCore.Mvc;
using TradingSim.Application.DTOs.Auth;
using TradingSim.Application.UseCases.Auth;

namespace TradingSim.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly SignupUseCase _signup;
    private readonly LoginUseCase _login;

    public AuthController(SignupUseCase signup, LoginUseCase login)
    {
        _signup = signup;
        _login = login;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequest req)
    {
        await _signup.ExecuteAsync(req);
        return Ok(new { message = "Signup successful" });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
    {
        var result = await _login.ExecuteAsync(req);
        return Ok(result);
    }
}