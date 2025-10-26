namespace DiscountSystem.ApiServer.Contracts;

public class UseCodeResponse
{
    public byte Result { get; set; }

    public UseCodeResponse(byte result)
    {
        Result = result;
    }
}
