using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TradingSim.Domain.Enums;

namespace TradingSim.Domain.Entities;

public sealed class PurchasePowerRequest
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    public string UserId { get; set; } = default!;
    public string UserEmail { get; set; } = default!;
    public decimal Amount { get; set; }
    
    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
