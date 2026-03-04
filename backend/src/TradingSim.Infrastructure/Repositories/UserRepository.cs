using MongoDB.Driver;
using TradingSim.Application.Interfaces.Repositories;
using TradingSim.Domain.Entities;
using TradingSim.Infrastructure.Mongo;

namespace TradingSim.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _col;

    public UserRepository(MongoContext ctx)
    {
        _col = ctx.Db.GetCollection<User>(Collections.Users);
    }

    public async Task<User?> GetByEmailAsync(string email)
        => await _col.Find(x => x.Email == email).FirstOrDefaultAsync();

    public async Task<User?> GetByIdAsync(string id)
        => await _col.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(User user)
        => await _col.InsertOneAsync(user);
}