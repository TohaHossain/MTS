using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradingSim.Application.DTOs.Power;
using TradingSim.Application.UseCases.Power;

namespace TradingSim.Api.Controllers;

[ApiController]
[Route("power")]
public sealed class PowerController : ControllerBase
{
    private readonly RequestPowerUseCase _requestPower;
    private readonly ListMyPowerRequestsUseCase _listMy;
    private readonly ListPendingPowerRequestsUseCase _listPending;
    private readonly ReviewPowerUseCase _review;
    private readonly GetBalanceUseCase _balance;

    public PowerController(
        RequestPowerUseCase requestPower,
        ListMyPowerRequestsUseCase listMy,
        ListPendingPowerRequestsUseCase listPending,
        ReviewPowerUseCase review,
        GetBalanceUseCase balance)
    {
        _requestPower = requestPower;
        _listMy = listMy;
        _listPending = listPending;
        _review = review;
        _balance = balance;
    }

    [HttpGet("balance")]
    [Authorize]
    public async Task<IActionResult> GetBalance()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var bal = await _balance.ExecuteAsync(id);
        return Ok(new { purchasePower = bal });
    }

    [HttpPost("request")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> RequestPower([FromBody] RequestPowerRequest req)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _requestPower.ExecuteAsync(id, req);
        return Ok(new { message = "Power requested successfully" });
    }

    [HttpGet("my")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<List<PowerRequestDto>>> GetMyRequests()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _listMy.ExecuteAsync(id));
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PowerRequestDto>>> GetPending()
    {
        return Ok(await _listPending.ExecuteAsync());
    }

    [HttpPost("{id}/review")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Review(string id, [FromBody] ReviewPowerRequest req)
    {
        await _review.ExecuteAsync(id, req);
        return Ok(new { message = "Request reviewed" });
    }
}
