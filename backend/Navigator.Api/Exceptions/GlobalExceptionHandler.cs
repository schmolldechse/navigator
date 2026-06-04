using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Navigator.Api.Mapping;
using Navigator.Observability;
using System.Diagnostics;

namespace Navigator.Api.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
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
        var correlationId = httpContext.Items[NavigatorCorrelation.CorrelationIdItemName]?.ToString();
        var traceId = Activity.Current?.TraceId.ToString();

        logger.LogError(
            exception,
            "Unhandled request exception returned {StatusCode}. CorrelationId: {CorrelationId}. TraceId: {TraceId}",
            statusCode,
            correlationId,
            traceId);

        var problemDetails = new ProblemDetails()
        {
            Status = statusCode,
            Title = "An error occurred while proccessing your request.",
            Detail = exception.Message,
        };
        problemDetails.Extensions["correlationId"] = correlationId;
        problemDetails.Extensions["traceId"] = traceId;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
