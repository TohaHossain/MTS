using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TradingSim.Infrastructure.Mongo;

public sealed class MongoContext
{
    public IMongoDatabase Db { get; }

    public MongoContext(IOptions<MongoOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        Db = client.GetDatabase(options.Value.Database);
    }
}