# Navigator.Observability

`Navigator.Observability` contains shared logging, tracing, metrics, and correlation helpers used by Navigator backend services.

## Responsibilities

- Configure Serilog from application configuration.
- Enrich logs with service name, environment, exception details, trace ID, and span ID.
- Configure OpenTelemetry tracing and metrics.
- Instrument ASP.NET Core, HTTP clients, runtime metrics, and Npgsql.
- Export telemetry to OTLP when enabled.
- Add request logging and correlation IDs for `Navigator.Api`.
- Add job activity and logging scopes for Quartz jobs in `Navigator.Daemon` and bootstrap work in `Navigator.Preflight`.

## Custom metrics

The shared `Navigator` meter emits low-cardinality job metrics for Prometheus-compatible backends:

```text
navigator_job_duration_seconds
navigator_job_failures_total
```

Both metrics use `job_name` and `job_outcome` tags. `FireInstanceId`, correlation IDs, trace IDs, and other per-run values are intentionally excluded from metric tags.

## Configuration

OTLP export can be configured through application settings:

```text
Observability:Otlp:Enabled
Observability:Otlp:Endpoint
```

The endpoint can also be supplied through:

```text
OTEL_EXPORTER_OTLP_ENDPOINT
```

Serilog sinks are configured through the standard `Serilog` configuration section in each host application.

## Correlation IDs

`NavigatorCorrelation` uses the request header:

```text
X-Correlation-ID
```

If no inbound correlation ID is present, the middleware creates one and returns it on the response. Incoming values are trimmed and capped to 128 characters.

## References

- [Serilog documentation](https://serilog.net/)
- [OpenTelemetry .NET documentation](https://opentelemetry.io/docs/languages/dotnet/)
- [Npgsql OpenTelemetry documentation](https://www.npgsql.org/doc/diagnostics.html)
- [ASP.NET Core logging documentation](https://learn.microsoft.com/aspnet/core/fundamentals/logging/)
