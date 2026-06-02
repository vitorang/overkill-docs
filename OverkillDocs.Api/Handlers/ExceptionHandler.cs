using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OverkillDocs.Core.Exceptions;
using System.Net;

namespace OverkillDocs.Api.Handlers;

public class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = GetStatusCode(exception.GetType());

        if (exception is CoreException coreException)
            statusCode = coreException.StatusCode;

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
}
