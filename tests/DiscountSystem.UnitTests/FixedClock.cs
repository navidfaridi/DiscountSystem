using DiscountSystem.Core.Application.Interfaces;

namespace DiscountSystem.UnitTests;
internal sealed class FixedClock : IClock
{
    public DateTime UtcNow { get; }
    public FixedClock(DateTime now) => UtcNow = now;
}
