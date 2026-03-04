using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingSim.Application.DTOs.Orders;
using TradingSim.Application.UseCases.Orders;

namespace TradingSim.Api.Controllers;

[ApiController]
[Route("orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly PlaceOrderUseCase _place;
    private readonly CancelOrderUseCase _cancel;
    private readonly ListOpenOrdersUseCase _open;

    public OrdersController(
        PlaceOrderUseCase place,
        CancelOrderUseCase cancel,
        ListOpenOrdersUseCase open)
    {
        _place = place;
        _cancel = cancel;
        _open = open;
    }

    [HttpPost]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<OrderDto>> Place([FromBody] PlaceOrderRequest req)
    {
        var userId = GetUserId();
        var order = await _place.ExecuteAsync(userId, req);
        return Ok(order);
    }

    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Cancel([FromRoute] string id)
    {
        var userId = GetUserId();
        await _cancel.ExecuteAsync(userId, id);
        return Ok(new { message = "Order cancelled" });
    }

    [HttpGet("open")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<List<OrderDto>>> Open()
    {
        var userId = GetUserId();
        return Ok(await _open.ExecuteAsync(userId));
    }

    private string GetUserId()
        => User.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? User.FindFirstValue(ClaimTypes.Name)
           ?? User.FindFirstValue("sub")
           ?? throw new Exception("Invalid token: user id missing");
}