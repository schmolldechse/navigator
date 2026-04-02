using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Navigator.Api.Mapping;

namespace Navigator.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            HttpRequestException => StatusCodes.Status502BadGateway,
            JsonException or MappingException => StatusCodes.Status422UnprocessableEntity,
            UnsupportedContentTypeException => StatusCodes.Status415UnsupportedMediaType,
            _ => StatusCodes.Status500InternalServerError
        };
        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails()
        {
            Status = statusCode,
            Title = "An error occurred while proccessing your request.",
            Detail = exception.Message,
        };
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
