using DiscountSystem.Core.Domain.Enums;
using DiscountSystem.Core.Domain.Exceptions;
using DiscountSystem.Core.Domain.ValueObjects;
using System;

namespace DiscountSystem.Core.Domain.Entites;

public class DiscountCode
{
    public Code Code { get; set; }
    public DiscountStatus Status { get; set; } = DiscountStatus.Available;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }

    public DiscountCode() { } 
    public DiscountCode(Code code, DateTime createdAtUtc)
    {
        Code = code;
        CreatedAtUtc = createdAtUtc;
    }

    public void Use(DateTime nowUtc)
    {
        if (Status != DiscountStatus.Available)
            throw new DomainException("Code cannot be used.");
        Status = DiscountStatus.Used;
        UsedAtUtc = nowUtc;
        // Raise domain event if needed
    }
}
