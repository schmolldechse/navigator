using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Navigator.Observability;

public class NavigatorCorrelation(RequestDelegate next)
{
    public const string CorrelationIdHeader = "X-Correlation-ID";
    public const string CorrelationIdItemName = "CorrelationId";
    private const int MaxCorrelationIdLength = 128;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context);
        context.Items[CorrelationIdItemName] = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }

    private static string ResolveCorrelationId(HttpContext context)
    {
        var inbound = context.Request.Headers[CorrelationIdHeader].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(inbound)) return Guid.NewGuid().ToString("n");

        inbound = inbound.Trim();
        return inbound.Length <= MaxCorrelationIdLength
            ? inbound
            : inbound[..MaxCorrelationIdLength];
    }
}
