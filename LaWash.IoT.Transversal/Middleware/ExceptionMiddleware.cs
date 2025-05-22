using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LaWash.IoT.Transversal;
public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (BadResponseWithMessage ex)
        {
            _logger.LogWarning(ex, "Controlled exception");

            await WriteErrorResponse(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected exception: {ex.Message}");

            await WriteErrorResponse(context, StatusCodes.Status500InternalServerError, $"An unexpected error occurred: {ex.Message}.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        var error = new ErrorResponseWithMessage
        {
            StatusCode = statusCode,
            Message = message
        };

        context.Response.ContentType = ContentType.Json;
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(error));
    }

}

