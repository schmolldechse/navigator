# Navigator.Data

`Navigator.Data` is the shared data layer for the Navigator backend. It owns Entity Framework Core mappings, database entities, repository implementations, generated Deutsche Bahn API models, and the metric builders used by `Navigator.Api` and `Navigator.Daemon`.

## Responsibilities

- Configure PostgreSQL, TimescaleDB, enum mappings, and spatial extensions through `DataContext`.
- Store stations, RIL100 identifiers, transport coverage, RIS IDs, journeys, and statistics snapshots.
- Provide repositories for stations, timetables, journeys, RIS IDs, and statistics.
- Call upstream Deutsche Bahn APIs through repository implementations.
- Build metric series for the frontend dashboard.

## Upstream Interfaces

The data layer calls these external interfaces:

- [RIS::Boards](https://developer-docs.deutschebahn.com/doku/apis/ris-boards-10686900) for arrivals and departures by station and time window.
- [RIS::Journeys](https://developer-docs.deutschebahn.com/doku/apis/ris-journeys-10582266) for single and batch journey records.
- [RIS::Stations](https://developer-docs.deutschebahn.com/doku/apis/ris-stations-10686906) for stop-place discovery by position.
- [StaDa](https://developers.deutschebahn.com/db-api-marketplace/apis/product) for station metadata.
- Deutsche Bahn's Vendo location search endpoint for station search.

External API credentials are read from environment variables by the repositories that need them. Do not commit real credentials.

## Database Layout

Navigator uses two main schemas:

- `core`: source-of-truth entities such as stations, RIS IDs, journeys, journey transports, stop places, and messages.
- `statistics`: snapshots, derived journey fact tables, projection backlog, and TimescaleDB continuous aggregates.

Journey data is stored in two layers:

1. Raw journey tables in the `core` schema keep the normalized source data:
   - `core.journeys`
   - `core.journey_transports`
   - `core.journey_stop_places`
2. Analytics fact tables in the `statistics` schema keep pre-shaped rows for TimescaleDB continuous aggregates:
   - `statistics.journey_event_quality_facts`
   - `statistics.journey_route_quality_facts`

The fact tables are intentionally derived data. They do not replace the raw journey tables and must not be treated as the source of truth.

## Journey Analytics Storage

TimescaleDB continuous aggregates work best when the aggregate query is a straightforward `time_bucket(...)` plus `GROUP BY` over a single fact source. Historical journey quality needs more shaping than that:

- joins between journeys, transports, and stop places
- origin and destination station resolution
- terminal delay selection per journey
- replacement transport detection

That shaping is done once when journeys are imported or backfilled. Continuous aggregates then count and sum already-prepared facts.

Current flow:

```text
RIS::Journeys
  -> EF journey graph
  -> core raw journey hypertables
  -> statistics fact hypertables
  -> TimescaleDB continuous aggregates
  -> API metric builders
```

`statistics.journey_event_quality_facts` contains one row per stop-place event and powers:

- `statistics.station_line_route_quality_hourly`

`statistics.journey_route_quality_facts` contains one row per journey and powers:

- `statistics.journey_route_quality_hourly`

The API reads from the hourly statistics views. Raw journey tables remain available for audits, reprocessing, and future analytics.

## Refresh And Compression Windows

Journey imports can write data several months in the past because RIS IDs may be discovered late in a timetable period or continued across operating dates. TimescaleDB policies therefore keep the active refresh window wider than a half-year timetable period:

- continuous aggregate refresh window: `210 days`
- compression policy for raw journey and fact hypertables: `240 days`

This keeps the current timetable period plus buffer uncompressed and refreshable. Compression starts only after the data is expected to be historically stable.

## Model Generation

When adding or updating C# models for Deutsche Bahn APIs, download the relevant OpenAPI specification from the [DB API Marketplace](https://developers.deutschebahn.com/db-api-marketplace/apis/product), then generate models with NSwag:

```sh
nswag openapi2csclient /input:<path-of-spec> /output:<output-file>.cs /namespace:Navigator.Data /JsonLibrary:SystemTextJson
```

Place generated RIS models in `Models/Ris`. Keep generated files reviewable and avoid mixing generated model changes with unrelated hand-written repository changes.

## References

- [Navigator.Api README](../Navigator.Api/README.md)
- [Navigator.Daemon README](../Navigator.Daemon/README.md)
- [TimescaleDB continuous aggregates](https://docs.timescale.com/use-timescale/latest/continuous-aggregates/)
- [Entity Framework Core documentation](https://learn.microsoft.com/ef/core/)
- [DB API Marketplace](https://developers.deutschebahn.com/db-api-marketplace/apis/product)
- [NSwag](https://github.com/RicoSuter/NSwag)
