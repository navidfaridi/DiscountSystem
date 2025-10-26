using DiscountSystem.Core.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;

namespace DiscountSystem.SignalRServer;

public class HubErrorFilter : IHubFilter
{
    private readonly ILogger<HubErrorFilter> _logger;
    public HubErrorFilter(ILogger<HubErrorFilter> logger) => _logger = logger;

    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext context,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(context);
        }
        catch (ValidationException ex)
        {
            var msg = string.Join(" | ", ex.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning(ex, "Validation error on {Hub}.{Method}: {Msg}",
                context.Hub.GetType().Name, context.HubMethodName, msg);
            throw new HubException(msg);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error on {Hub}.{Method}: {Msg}",
                context.Hub.GetType().Name, context.HubMethodName, ex.Message);
            throw new HubException(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error on {Hub}.{Method}",
                context.Hub.GetType().Name, context.HubMethodName);
            throw new HubException("An unexpected error occurred.");
        }
    }
}
