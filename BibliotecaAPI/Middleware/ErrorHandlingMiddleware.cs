using BibliotecaAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Middleware;

public sealed class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException exception)
        {
            logger.LogWarning(exception, "Recurso não encontrado: {Message}", exception.Message);
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status404NotFound,
                "Recurso não encontrado",
                exception.Message);
        }
        catch (ConflictException exception)
        {
            logger.LogWarning(exception, "Conflito de negócio: {Message}", exception.Message);
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status409Conflict,
                "Conflito de negócio",
                exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Erro inesperado ao processar a requisição.");
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Erro interno do servidor",
                "Ocorreu um erro inesperado.");
        }
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Detail = detail
        });
    }
}
