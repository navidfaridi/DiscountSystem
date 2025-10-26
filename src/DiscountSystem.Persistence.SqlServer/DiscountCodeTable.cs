using DiscountSystem.Core.Domain.Enums;
using DiscountSystem.Core.Domain.ValueObjects;

namespace DiscountSystem.Persistence.SqlServer;

public class DiscountCodeTable
{
    public Code Id { get; set; } = null!;
    public DiscountStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
}
