namespace DiscountSystem.ApiServer.Contracts;

public class ApiErrorResponse
{
    public int ErrorCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Path { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

