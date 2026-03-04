using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingSim.Application.DTOs.Instruments;
using TradingSim.Application.UseCases.Instruments;

namespace TradingSim.Api.Controllers;

[ApiController]
[Route("instruments")]
public sealed class InstrumentsController : ControllerBase
{
    private readonly ListInstrumentsUseCase _list;
    private readonly CreateInstrumentUseCase _create;

    public InstrumentsController(ListInstrumentsUseCase list, CreateInstrumentUseCase create)
    {
        _list = list;
        _create = create;
    }

    [HttpGet]
    [Authorize] // both Client + Admin
    public async Task<ActionResult<List<InstrumentDto>>> GetAll()
        => Ok(await _list.ExecuteAsync());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateInstrumentRequest req)
    {
        await _create.ExecuteAsync(req);
        return Ok(new { message = "Instrument created" });
    }
}