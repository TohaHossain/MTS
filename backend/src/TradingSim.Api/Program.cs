using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["Mongo:ConnectionString"];
var databaseName = builder.Configuration["Mongo:Database"];

var mongoClient = new MongoClient(connectionString);
var database = mongoClient.GetDatabase(databaseName);

// optional seed
// await SeedData.SeedAsync(database);
await SeedData.SeedAsync(database);
var app = builder.Build();

app.MapGet("/", () => "TradingSim API Running");

app.Run();