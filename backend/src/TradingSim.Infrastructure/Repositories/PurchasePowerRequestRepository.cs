using MongoDB.Driver;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Domain.Entities;
using TradingSim.Domain.Enums;
using TradingSim.Infrastructure.Mongo;

namespace TradingSim.Infrastructure.Repositories;

public sealed class PurchasePowerRequestRepository : IPurchasePowerRequestRepository
{
    private readonly IMongoCollection<PurchasePowerRequest> _col;

    public PurchasePowerRequestRepository(MongoContext ctx)
    {
        _col = ctx.Db.GetCollection<PurchasePowerRequest>(Collections.PurchasePowerRequests);
    }

    public async Task<List<PurchasePowerRequest>> GetByUserIdAsync(string userId)
        => await _col.Find(x => x.UserId == userId).SortByDescending(x => x.CreatedUtc).ToListAsync();

    public async Task<List<PurchasePowerRequest>> GetPendingAsync()
        => await _col.Find(x => x.Status == RequestStatus.Pending).SortByDescending(x => x.CreatedUtc).ToListAsync();

    public async Task<PurchasePowerRequest?> GetByIdAsync(string id)
        => await _col.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(PurchasePowerRequest request)
        => await _col.InsertOneAsync(request);

    public async Task UpdateAsync(PurchasePowerRequest request)
        => await _col.ReplaceOneAsync(x => x.Id == request.Id, request);
}
