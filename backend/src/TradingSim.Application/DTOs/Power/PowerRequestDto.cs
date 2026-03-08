using TradingSim.Domain.Enums;

namespace TradingSim.Application.DTOs.Power;

public sealed class PowerRequestDto
{
    public string Id { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string UserEmail { get; set; } = default!;
    public decimal Amount { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedUtc { get; set; }
}
