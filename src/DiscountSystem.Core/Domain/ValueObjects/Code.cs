using DiscountSystem.Core.Domain.Exceptions;

namespace DiscountSystem.Core.Domain.ValueObjects;

public sealed record Code
{
    public string Value { get; }
    public Code(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("Code is empty.");
        if (value.Length is < 7 or > 8) throw new DomainException("Code length must be 7-8.");
        
        Value = value;
    }
    public override string ToString() => Value;
}