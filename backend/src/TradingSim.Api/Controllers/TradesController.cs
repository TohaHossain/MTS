using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingSim.Application.DTOs.Trades;
using TradingSim.Application.UseCases.Trades;

namespace TradingSim.Api.Controllers;

[ApiController]
[Route("trades")]
public sealed class TradesController : ControllerBase
{
    private readonly GetMyTradesUseCase _my;
    private readonly GetAllTradesUseCase _all;

    public TradesController(GetMyTradesUseCase my, GetAllTradesUseCase all)
    {
        _my = my;
        _all = all;
    }

    [HttpGet("me")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<List<TradeDto>>> MyTrades()
    {
        var userId = GetUserId();
        return Ok(await _my.ExecuteAsync(userId));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<TradeDto>>> AllTrades()
        => Ok(await _all.ExecuteAsync());

    private string GetUserId()
        => User.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? User.FindFirstValue(ClaimTypes.Name)
           ?? User.FindFirstValue("sub")
           ?? throw new Exception("Invalid token: user id missing");
}