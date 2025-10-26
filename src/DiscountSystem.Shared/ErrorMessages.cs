namespace DiscountSystem.Shared;

public static class ErrorMessages
{
    public static ErrorMessage CountOutOfRange = new ErrorMessage(1, "Count must be between 1 and 2000.");

    public static ErrorMessage LengthOutOfRange = new ErrorMessage(1, "Length must be 7 or 8.");

    public static ErrorMessage CodeRequired = new ErrorMessage(1, "Code is required.");

    public static ErrorMessage CodeLengthInvalid = new ErrorMessage(1, "Code length must be between 7 and 8.");

    public static ErrorMessage CodeInvalidCharacters = new ErrorMessage(1, "Code must contain only letters and digits.");
}
