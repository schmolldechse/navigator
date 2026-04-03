using Serilog.Context;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string _correlationIdHeader = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[_correlationIdHeader].FirstOrDefault() ?? Guid.NewGuid().ToString();
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[_correlationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}