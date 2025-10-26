namespace DiscountSystem.ApiServer.Contracts;

public class GenerateResponse
{
    public GenerateResponse(bool value)
    {
        this.Success = value;
    }
    public bool Success { get; set; }
}
