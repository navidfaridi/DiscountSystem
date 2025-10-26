using DiscountSystem.Core.Application.Commands;
using DiscountSystem.Core.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DiscountSystem.SignalRServer;

public class DiscountHub : Hub
{
    private readonly IDiscountAppService _app;
    public DiscountHub(IDiscountAppService app) => _app = app;

    public async Task<bool> Generate(ushort count, byte length)
    {
        var ct = Context.ConnectionAborted;
        var res = await _app.GenerateAsync(new GenerateCodesCommand(count, length), ct);
        return res.Success;
    }

    public async Task<byte> UseCode(string code)
    {
        var ct = Context.ConnectionAborted;
        var result = await _app.UseAsync(new UseCodeCommand(code.Trim().ToUpperInvariant()), ct);
        return (byte)result;
    }
}