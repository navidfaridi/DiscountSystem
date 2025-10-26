namespace DiscountSystem.Core.Domain.Enums;

public enum UseCodeResult : byte
{
    Success = 0,
    NotFound = 1,
    AlreadyUsed = 2,
    Invalid = 3,
    UnknownError = 4
}