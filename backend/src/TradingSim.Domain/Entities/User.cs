using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TradingSim.Domain.Enums;

namespace TradingSim.Domain.Entities;

public sealed class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public UserRole Role { get; set; } = UserRole.Client;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}