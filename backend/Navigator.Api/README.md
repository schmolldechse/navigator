# Navigator.Api

`Navigator.Api` is the public ASP.NET Core HTTP API for Navigator. It exposes station lookup, timetable, journey, and statistics endpoints and serves the OpenAPI/Scalar documentation used by the frontend API generator.

## Responsibilities

- Serve `/api/v1/stations` endpoints for search, nearby stations, EVA lookup, station batches, and gathering metadata.
- Serve `/api/v1/timetable` endpoints for RIS board arrivals and departures.
- Serve `/api/v1/journey` endpoints for single and batch journey lookup.
- Serve `/api/v1/statistics/metrics` for dashboard metric series.
- Normalize JSON output with snake-case enum values and local date/time conversion.
- Apply shared observability, request logging, correlation IDs, and global exception handling.

## Configuration

The API uses the shared `Navigator.Data` services and requires a PostgreSQL/TimescaleDB connection string:

```text
ConnectionStrings:DefaultConnection
```

For local development, the default database container expects:

```text
Host=localhost;Port=5432;Database=navigator;Username=navigator;Password=password
```

External Deutsche Bahn API credentials are read from environment variables:

```text
BOARDS_CLIENT_ID
BOARDS_API_KEY
JOURNEYS_CLIENT_ID
JOURNEYS_API_KEY
```

## Development

Run from the repository root:

```sh
dotnet run --project backend/Navigator.Api
```

The default HTTP launch profile listens on:

```text
http://localhost:5019
```

Useful endpoints:

```text
http://localhost:5019/swagger
http://localhost:5019/swagger.json
http://localhost:5019/openapi/v1.json
```

The frontend API client is generated from `http://localhost:5019/swagger.json`.

## References

- [Navigator.Data README](../Navigator.Data/README.md)
- [Navigator.Observability README](../Navigator.Observability/README.md)
- [ASP.NET Core OpenAPI documentation](https://learn.microsoft.com/aspnet/core/fundamentals/openapi/overview)
- [Scalar ASP.NET Core integration](https://github.com/scalar/scalar/tree/main/integrations/aspnetcore)
- [RIS::Boards documentation](https://developer-docs.deutschebahn.com/doku/apis/ris-boards-10686900)
- [RIS::Journeys documentation](https://developer-docs.deutschebahn.com/doku/apis/ris-journeys-10582266)
