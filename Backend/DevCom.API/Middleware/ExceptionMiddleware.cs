using System.Net;
using System.Text.Json;

namespace DevCom.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Excecao nao tratada: {Message}", ex.Message);
            if (context.Response.HasStarted)
            {
                logger.LogWarning("Resposta ja iniciada — impossivel retornar erro JSON.");
                return;
            }
            await WriteErrorAsync(context, ex);
        }
    }

    private static Task WriteErrorAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            KeyNotFoundException        => (HttpStatusCode.NotFound,            ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden,           ex.Message),
            InvalidOperationException   => (HttpStatusCode.BadRequest,          ex.Message),
            _                           => (HttpStatusCode.InternalServerError, "Ocorreu um erro interno.")
        };
        context.Response.StatusCode  = (int)statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));
    }
}