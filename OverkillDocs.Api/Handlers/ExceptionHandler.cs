using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OverkillDocs.Core.Exceptions;
using System.Net;

namespace OverkillDocs.Api.Handlers;

public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = GetStatusCode(exception.GetType());

        if (exception is CoreException coreException)
            statusCode = coreException.StatusCode;

        LogException(httpContext, exception, statusCode);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(statusCode),
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    public static int GetStatusCode(Type exceptionType)
    {
        if (typeof(CoreException).IsAssignableFrom(exceptionType))
        {
            var coreException = (CoreException)Activator.CreateInstance(exceptionType, string.Empty)!;
            return coreException.StatusCode;
        }

        if (exceptionType == typeof(NotImplementedException))
            return (int)HttpStatusCode.NotImplemented;

        return (int)HttpStatusCode.InternalServerError;
    }

    public static string GetTitle(int statusCode) => statusCode switch
    {
        400 => "Requisição inválida",
        401 => "Não autorizado",
        404 => "Recurso não encontrado",
        409 => "Conflito",
        500 => "Erro interno",
        501 => "Não implementado",
        _ => "Erro interno do servidor"
    };

    private void LogException(HttpContext context, Exception exception, int statusCode)
    {
        var path = context.Request.Path;
        var method = context.Request.Method;

        if (statusCode >= 500)
        {
            logger.LogError(
                exception,
                "Falha na requisição {Method}({StatusCode}): {Path}",
                method,
                statusCode,
                path
            );
        }
        else
        {
            logger.LogWarning(
                "Requisição inválida: {Method}({StatusCode}): {Path} | Mensagem: {Message}",
                method,
                statusCode,
                path,
                exception.Message
            );
        }
    }
}
