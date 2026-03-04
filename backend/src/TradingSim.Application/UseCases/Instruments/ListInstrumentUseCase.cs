using TradingSim.Application.DTOs.Instruments;
using TradingSim.Application.Interfaces.Repositories;

namespace TradingSim.Application.UseCases.Instruments;

public sealed class ListInstrumentsUseCase
{
    private readonly IInstrumentRepository _repo;

    public ListInstrumentsUseCase(IInstrumentRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<InstrumentDto>> ExecuteAsync()
    {
        var list = await _repo.GetAllAsync();

        return list.Select(i => new InstrumentDto
        {
            Id = i.Id,
            Symbol = i.Symbol,
            Name = i.Name,
            LastPrice = i.LastPrice
        }).ToList();
    }
}