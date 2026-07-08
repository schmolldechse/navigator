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
- `statistics`: snapshots, rebuildable aggregate rollups, detail views, refresh progress, and TimescaleDB continuous aggregates.

Journey data is stored in two layers:

1. Raw journey tables in the `core` schema keep the normalized source data:
   - `core.journeys`
   - `core.journey_transports`
   - `core.journey_stop_places`
2. Rebuildable aggregate rollup tables in the `statistics` schema keep hourly aggregate rows for TimescaleDB continuous aggregates:
   - `statistics.event_quality_hourly_rollups`
   - `statistics.journey_quality_hourly_rollups`

The rollup tables are intentionally derived data. They do not replace the raw journey tables and must not be treated as the source of truth.

## Journey Analytics Storage

TimescaleDB continuous aggregates work best when the aggregate query is a straightforward `time_bucket(...)` plus `GROUP BY` over a bounded aggregate source. Historical journey quality needs more shaping than that:

- joins between journeys, transports, and stop places
- origin and destination station resolution
- exact stop-event time and delay selection
- destination delay and journey outcome selection
- replacement transport detection

That shaping is recomputed from `core` into hourly rollups by `StatisticsAggregateRefreshJob`. Newly imported journeys, including old journeys discovered through reactivated RIS IDs, mark exact hourly windows in `statistics.statistics_refresh_queue`.

The daemon calls `statistics.recompute_quality_hourly_rollups`, which builds one shared temporary workset for the requested window and writes event and journey aggregate rows from that workset. Continuous aggregates then sum those hourly rollups.

The unified recompute path still writes only rebuildable aggregates. It does not create durable per-event facts, durable per-journey facts, or a projection backlog.

Current flow:

```text
RIS::Journeys
  -> EF journey graph
  -> core raw journey hypertables
  -> statistics hourly aggregate rollups
  -> statistics detail views backed by core
  -> TimescaleDB continuous aggregates
  -> API metric builders
```

`statistics.event_quality_hourly_rollups` contains hourly aggregate rows by station, schedule type, administration, transport, line, route, and replacement dimensions. It powers station, network, administration, station-line, and delay distribution continuous aggregates.

`statistics.journey_quality_hourly_rollups` contains hourly aggregate rows by administration, transport, line, route, journey number, and replacement dimensions. It powers network, administration, line, journey-number, and journey-outcome continuous aggregates.

Detail queries read views derived from `core`:

- `statistics.station_journey_event_details`
- `statistics.journey_quality_details`

Metric queries read API-specific continuous aggregates:

- `statistics.network_event_quality_hourly`
- `statistics.network_event_delay_distribution_hourly`
- `statistics.network_journey_quality_hourly`
- `statistics.station_event_quality_hourly`
- `statistics.station_administration_quality_hourly`
- `statistics.line_event_quality_hourly`
- `statistics.station_line_quality_hourly`
- `statistics.journey_administration_quality_hourly`
- `statistics.line_journey_quality_hourly`
- `statistics.journey_number_quality_hourly`
- `statistics.network_journey_outcome_hourly`

The API reads from the hourly statistics views. Raw journey tables remain available for audits, reprocessing, and future analytics.

## Refresh And Compression Windows

Journey imports can write data in the past because RIS IDs may be discovered late in a timetable period or continued across operating dates. The target system handles this with explicit bounded recompute windows plus CAGG refreshes:

- `StatisticsAggregateRefreshJob` refreshes hot windows continuously and rotates through a catch-up horizon for late data.
- Newly imported journeys mark exact old windows in `statistics.statistics_refresh_queue`; queued windows are consumed by `StatisticsAggregateRefreshJob` even when they are older than the catch-up horizon.
- Continuous aggregate policies refresh the last 7 days as a safety net.

The `core` schema remains the source of truth. Statistics refresh code must not mutate `core` tables.

## Statistics Refresh Queue Checks

Pending or failed queued windows:

```sql
SELECT status, count(*)
FROM statistics.statistics_refresh_queue
GROUP BY status
ORDER BY status;

SELECT *
FROM statistics.statistics_refresh_queue
WHERE status IN ('PENDING', 'FAILED')
ORDER BY window_start
LIMIT 50;
```

Recently protected reactivated RIS IDs:

```sql
SELECT *
FROM statistics.ris_id_reactivation_holds
WHERE protect_until > now()
ORDER BY protect_until DESC
LIMIT 50;
```

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
