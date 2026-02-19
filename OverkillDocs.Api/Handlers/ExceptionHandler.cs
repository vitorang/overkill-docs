using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OverkillDocs.Core.Exceptions;
using System.Net;

namespace OverkillDocs.Api.Handlers
{
    public class ExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            
            var statusCode = exception switch
            {
                CoreException e => e.StatusCode,
                NotImplementedException => (int)HttpStatusCode.NotImplemented,
                Exception => (int)HttpStatusCode.InternalServerError
            };

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

        private static string GetTitle(int statusCode) => statusCode switch
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
}
