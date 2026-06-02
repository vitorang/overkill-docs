using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OverkillDocs.Api.Handlers;
using OverkillDocs.Core.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OverkillDocs.Api.Filters;

public class StatusResponseFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        AuthenticatedResults(operation, context);
        SuccessResults(operation, context);
        ErrorResults(operation, context);
    }

    private static void AuthenticatedResults(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAllowAnonymous = context.MethodInfo.DeclaringType?
            .GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length != 0 == true
            || context.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length != 0;

        if (!hasAllowAnonymous)
        {
            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add("401", new OpenApiResponse
                {
                    Description = ExceptionHandler.GetTitle(401),
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository)
                        }
                    }
                });
            }
        }
    }

    private static void SuccessResults(OpenApiOperation operation, OperationFilterContext context)
    {
        var returnType = context.MethodInfo.ReturnType;

        if (returnType.IsGenericType &&
            (returnType.GetGenericTypeDefinition() == typeof(Task<>) ||
             returnType.GetGenericTypeDefinition() == typeof(ValueTask<>)))
        {
            returnType = returnType.GetGenericArguments()[0];
        }

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ActionResult<>))
            returnType = returnType.GetGenericArguments()[0];


        if (returnType == typeof(NoContentResult))
        {
            operation.Responses.Remove("200");
            if (!operation.Responses.ContainsKey("204"))
                operation.Responses.Add("204", new OpenApiResponse { Description = "Sem conteúdo" });
        }
        else if (returnType == typeof(CreatedResult) || returnType == typeof(CreatedAtActionResult))
        {
            operation.Responses.Remove("200");
            if (!operation.Responses.ContainsKey("201"))
                operation.Responses.Add("201", new OpenApiResponse { Description = "Criado" });
        }
    }

    private static void ErrorResults(OpenApiOperation operation, OperationFilterContext context)
    {
        var attribute = context.MethodInfo
            .GetCustomAttributes(typeof(ProducesStatusForAttribute), inherit: true)
            .Cast<ProducesStatusForAttribute>()
            .FirstOrDefault();

        if (attribute != null)
        {
            foreach (var exceptionType in attribute.ExceptionTypes)
            {
                int statusCode = ExceptionHandler.GetStatusCode(exceptionType);
                string title = ExceptionHandler.GetTitle(statusCode);
                var statusCodeStr = statusCode.ToString();

                if (!operation.Responses.ContainsKey(statusCodeStr))
                {
                    operation.Responses.Add(statusCodeStr, new OpenApiResponse
                    {
                        Description = title,
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository)
                            }
                        }
                    });
                }
            }
        }
    }
}
