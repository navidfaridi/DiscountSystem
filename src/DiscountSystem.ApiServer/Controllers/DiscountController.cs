using DiscountSystem.ApiServer.Contracts;
using DiscountSystem.Core.Application.Commands;
using DiscountSystem.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiscountSystem.ApiServer.Controllers;

[ApiController]
[Route("[controller]")]
public class DiscountController : ControllerBase
{

    private readonly ILogger<DiscountController> _logger;
    private readonly IDiscountAppService _app;

    public DiscountController(ILogger<DiscountController> logger, IDiscountAppService discountAppService)
    {
        _logger = logger;
        this._app = discountAppService;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<GenerateResponse>> Generate([FromBody] GenerateRequest request, CancellationToken ct)
    {
        var result = await _app.GenerateAsync(
            new GenerateCodesCommand(request.Count, request.Length), ct);

        return Ok(new GenerateResponse(result.Success));
    }

    [HttpPost("use")]
    public async Task<ActionResult<UseCodeResponse>> Use([FromBody] UseCodeRequest request, CancellationToken ct)
    {
        var code = request.Code.Trim().ToUpperInvariant();

        var result = await _app.UseAsync(new UseCodeCommand(code), ct);
        return Ok(new UseCodeResponse((byte)result));
    }

}
