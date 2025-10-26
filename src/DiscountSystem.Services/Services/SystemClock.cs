using DiscountSystem.Core.Application.Interfaces;

namespace DiscountSystem.Services.Services;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
