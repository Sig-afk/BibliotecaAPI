using BibliotecaAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI;

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
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            await WriteProblemDetails(context, StatusCodes.Status404NotFound, "Recurso não encontrado", ex.Message);
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning(ex.Message);
            await WriteProblemDetails(context, StatusCodes.Status409Conflict, "Conflito de negócio", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado.");
            await WriteProblemDetails(context, StatusCodes.Status500InternalServerError, "Erro interno do servidor", "Ocorreu um erro inesperado.");
        }
    }

    private static async Task WriteProblemDetails(HttpContext context, int statusCode, string title, string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Detail = detail
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}
