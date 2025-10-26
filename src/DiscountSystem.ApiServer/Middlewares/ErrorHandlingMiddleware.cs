using DiscountSystem.Core.Domain.Exceptions;
using DiscountSystem.Shared;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace DiscountSystem.ApiServer.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error: {Errors}", string.Join("; ", ex.Errors.Select(e => e.ErrorMessage)));

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors.Select(e => new ErrorMessage(400, e.ErrorMessage)).ToList();
            var result = JsonSerializer.Serialize(errors);
            await context.Response.WriteAsync(result);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error: {Message}", ex.Message);

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new ErrorMessage(400, ex.Message));
            await context.Response.WriteAsync(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new ErrorMessage(500, "An unexpected error occurred."));
            await context.Response.WriteAsync(result);
        }
    }
}