using TradingSim.Application.DTOs.Instruments;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Domain.Entities;

namespace TradingSim.Application.UseCases.Instruments;

public sealed class CreateInstrumentUseCase
{
    private readonly IInstrumentRepository _repo;

    public CreateInstrumentUseCase(IInstrumentRepository repo)
    {
        _repo = repo;
    }

    public async Task ExecuteAsync(CreateInstrumentRequest request)
    {
        var existing = await _repo.GetBySymbolAsync(request.Symbol);
        if (existing != null)
            throw new Exception("Instrument already exists.");

        var instrument = new Instrument
        {
            Symbol = request.Symbol,
            Name = request.Name,
            LastPrice = request.LastPrice,
            MaxQuantity = request.MaxQuantity
        };

        await _repo.CreateAsync(instrument);
    }
}