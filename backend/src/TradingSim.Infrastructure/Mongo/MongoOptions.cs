namespace TradingSim.Infrastructure.Mongo;

public sealed class MongoOptions
{
    public string ConnectionString { get; set; } = default!;
    public string Database { get; set; } = default!;
}