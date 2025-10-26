
using DiscountSystem.Core.Domain.Enums;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DiscountSystem.UnitTests")]

namespace DiscountSystem.Services.Services;

internal class SimpleDiscountCode
{
    public string Code { get; set; } = null!;
    public UseCodeResult Status { get; set; }
}
