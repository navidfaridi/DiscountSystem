namespace DiscountSystem.Shared;

public class ErrorMessage
{
    public ErrorMessage(int code, string message)
    {
        this.Message = message;
        this.ErrorCode = code;
    }
    public int ErrorCode { get; set; }
    public string Message { get; set; } = string.Empty;
}
