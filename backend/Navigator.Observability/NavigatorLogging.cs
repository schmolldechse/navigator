using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;

namespace Navigator.Observability;

public static class NavigatorLogging
{
    private const string ActivitySourceName = "Navigator";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public static IHostApplicationBuilder AddNavigatorObservability(
        this IHostApplicationBuilder builder,
        string serviceName)
    {
        builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.With<ActivityTraceEnricher>()
            .Enrich.WithProperty("service_name", serviceName)
            .Enrich.WithProperty("env", builder.Environment.EnvironmentName));

        var otlpEndpoint = builder.Configuration["Observability:Otlp:Endpoint"]
            ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        var otlpEnabled = builder.Configuration.GetValue<bool?>("Observability:Otlp:Enabled")
            ?? !string.IsNullOrWhiteSpace(otlpEndpoint);

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName)
                .AddAttributes([
                    new KeyValuePair<string, object>("deployment.environment", builder.Environment.EnvironmentName)
                ]))
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(ActivitySourceName)
                    .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                    .AddHttpClientInstrumentation(options => options.RecordException = true)
                    .AddNpgsql();

                if (otlpEnabled && Uri.TryCreate(otlpEndpoint, UriKind.Absolute, out var endpoint))
                    tracing.AddOtlpExporter(options => options.Endpoint = endpoint);
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddRuntimeInstrumentation()
                    .AddNpgsqlInstrumentation()
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter("System.Net.Http");

                if (otlpEnabled && Uri.TryCreate(otlpEndpoint, UriKind.Absolute, out var endpoint))
                    metrics.AddOtlpExporter(options => options.Endpoint = endpoint);
            });

        return builder;
    }

    public static IApplicationBuilder UseNavigatorRequestLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RoutePattern} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.GetLevel = (httpContext, _, exception) =>
            {
                if (exception is not null || httpContext.Response.StatusCode >= StatusCodes.Status500InternalServerError)
                    return LogEventLevel.Error;
                return httpContext.Response.StatusCode >= StatusCodes.Status400BadRequest
                    ? LogEventLevel.Warning
                    : LogEventLevel.Information;
            };
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                var routePattern = httpContext.GetEndpoint() is RouteEndpoint routeEndpoint
                    ? routeEndpoint.RoutePattern.RawText
                    : httpContext.Request.Path.Value;
                var activity = Activity.Current;

                diagnosticContext.Set("RoutePattern", routePattern ?? "unknown");
                diagnosticContext.Set("RequestPath", httpContext.Request.Path.Value ?? string.Empty);
                diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
                diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);
                diagnosticContext.Set("CorrelationId", httpContext.Items[NavigatorCorrelation.CorrelationIdItemName]);
                diagnosticContext.Set("TraceId", activity?.TraceId.ToString());
                diagnosticContext.Set("SpanId", activity?.SpanId.ToString());
                diagnosticContext.Set("EventType", "http_request");
            };
        });

        return app;
    }

    public static Activity? StartJobActivity(string jobName, string? fireInstanceId = null)
    {
        var activity = ActivitySource.StartActivity(jobName, ActivityKind.Internal);
        activity?.SetTag("job.name", jobName);
        if (!string.IsNullOrWhiteSpace(fireInstanceId))
            activity?.SetTag("job.fire_instance_id", fireInstanceId);
        return activity;
    }

    public static IDisposable? BeginJobScope(
        this Microsoft.Extensions.Logging.ILogger logger,
        string jobName,
        string? fireInstanceId = null) => logger.BeginScope(new Dictionary<string, object?>
        {
            ["JobName"] = jobName,
            ["FireInstanceId"] = fireInstanceId,
            ["EventType"] = "quartz_job"
        });

    public static async Task RunJobAsync(
        this Microsoft.Extensions.Logging.ILogger logger,
        string jobName,
        string? fireInstanceId,
        Func<Task> executeAsync)
    {
        using var activity = StartJobActivity(jobName, fireInstanceId);
        using var scope = logger.BeginJobScope(jobName, fireInstanceId);
        var startedAt = Stopwatch.GetTimestamp();

        logger.LogInformation("Starting job {JobName}.", jobName);
        try
        {
            await executeAsync();
            logger.LogInformation(
                "Completed job {JobName} in {ElapsedMilliseconds}ms.",
                jobName,
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Failed job {JobName} after {ElapsedMilliseconds}ms.",
                jobName,
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds);
            throw;
        }
    }

    private sealed class ActivityTraceEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var activity = Activity.Current;
            if (activity is null) return;

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", activity.TraceId.ToString()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", activity.SpanId.ToString()));
        }
    }
}
