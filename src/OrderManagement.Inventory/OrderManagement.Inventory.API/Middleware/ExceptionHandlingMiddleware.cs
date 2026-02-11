using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OrderManagement.Inventory.Application.Exceptions;

namespace OrderManagement.Inventory.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        string title = "An unexpected error occurred.";
        string detail = exception.Message;

        switch (exception)
        {
            case StockItemNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                title = "Stock item not found.";
                break;
        }

        _logger.LogError(
            exception,
            "Unhandled exception caught by middleware. StatusCode: {StatusCode}, Title: {Title}",
            (int)statusCode,
            title);

        var problemDetails = new
        {
            type = $"https://httpstatuses.com/{(int)statusCode}",
            title,
            status = (int)statusCode,
            detail
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        var payload = JsonSerializer.Serialize(problemDetails, SerializerOptions);
        await context.Response.WriteAsync(payload);
    }
}

