using TradingSim.Domain.Entities;

namespace TradingSim.Application.Interfaces.Repositories;

public interface IInstrumentRepository
{
    Task<List<Instrument>> GetAllAsync();
    Task<Instrument?> GetBySymbolAsync(string symbol);
    Task CreateAsync(Instrument instrument);
    Task UpdateAsync(Instrument instrument);
}