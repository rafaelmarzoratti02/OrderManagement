using System.Net;
using System.Text.Json;
using OrderManagement.Inventory.Application.Exceptions;

namespace OrderManagement.Inventory.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
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

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
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

