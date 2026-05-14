# Navigator Journey Metrics Reference

## Purpose

Use this reference when designing Navigator dashboard metrics, PostgreSQL materialized views, EF Core view entities, API DTOs, or `StatisticsRepository` support for journey analytics. Keep dashboard output transparent for local public transportation: show raw counts and denominators alongside rates, keep transport/station filters visible, and avoid graph types that make cancellations, delay samples, or sparse data look smoother than they are.

## Dashboard Shape

Navigator metrics usually belong to one of two dashboard surfaces:

- General: all stations and transports combined by default, with transport filters. Good for national/system-wide trends, transport mix, operator comparisons, journey type share, cancellation rate, delay severity, platform changes, and disruption causes.
- Station-specific: one EVA station or a small EVA set. Good for local arrivals/departures, station delay, punctuality, platform-change hotspots, cancellation hotspots, and station disruption causes.

Useful graph defaults:

- KPI plus line chart: total journeys, active RIS IDs, database growth, latest cancellation rate.
- Line chart or stacked area: hourly arrivals/departures, punctuality, delay rate, cancellation rate, platform-change rate.
- Stacked bar or 100% stacked area: delay buckets, journey type share, message type share.
- Ranked bar or Pareto bar: transport mix, operator reliability, disruption causes, station hotspots.
- Map heatmap: station counts, cancellations, delay averages, platform-change hotspots, disruption hotspots.
- Calendar/hour heatmap: selected-station delay, punctuality, cancellations, platform changes.
- Table with sparkline: operator and station transparency views where exact counts matter.

For German local public transport transparency, a local-transit preset should normally include `REGIONAL_TRAIN`, `CITY_TRAIN`, `SUBWAY`, `TRAM`, `BUS`, and `FERRY`. General metrics should still support all captured `core.transport_type` values unless the user asks for a local-only default.

## Source Schema

Journey facts live in PostgreSQL `core` tables created by raw SQL migrations. Journey tables are partitioned by `date`, and partition-friendly analytics must preserve that key in joins.

- `core.journeys`: journey header keyed by `(id, date)`. Useful columns: `id`, `date`, `inserted_at`, `administration_id`, `cancelled`, `journey_type`.
- `core.journey_administrations`: operator/admin lookup keyed by `id`. Useful columns: `administration_id`, `operator_code`, `operator_name`.
- `core.journey_transports`: one transport row per journey keyed by `(journey_id, date)`. Useful columns: `transport_type`, `replacement_transport_type`, `category`, `category_internal`, `journey_description`, `label`, `line`, `number`.
- `core.journey_stop_places`: stop-level events keyed by `(id, date)`. Useful columns: `journey_id`, `date`, `schedule_type`, `station_eva_number`, `cancelled`, `additional`, `demand`, `no_passenger_change`, `planned_time`, `actual_time`, `time_type`, generated `delay`, `planned_platform`, `actual_platform`.
- `core.journey_messages`: journey-level messages keyed by `(id, date)`. Useful columns: `journey_id`, `date`, `message_type`, `code`, `text`, `text_short`, `disruption_cause`, `disruption_effect`, `note_category`.
- `core.journey_stop_place_messages`: bridge keyed by `(journey_stop_place_id, journey_message_id, date)` linking stop places to messages.

Always join partitioned journey tables by the date-aware composite keys:

```sql
JOIN core.journey_transports AS transport
  ON stop_place.journey_id = transport.journey_id
 AND stop_place.date = transport.date
```

Use the same date-aware pattern for `journeys`, `journey_messages`, and `journey_stop_place_messages`. Do not join journey facts only by `journey_id`, message id, or stop-place id.

## Enum Literals

PostgreSQL enum values live under `core` and are mapped in C# with `PgName` values.

- `core.journey_type`: `REGULAR`, `REPLACEMENT`, `RELIEF`, `EXTRA`
- `core.schedule_type`: `ARRIVAL`, `DEPARTURE`
- `core.time_type`: `SCHEDULE`, `PREVIEW`, `REAL`
- `core.message_type`: `ATTRIBUTE`, `DISRUPTION`, `NOTE`, `RIS_CAUSE`, `RIS_QUALITY_DEVIATION`
- `core.transport_type`: `UNKNOWN`, `HIGH_SPEED_TRAIN`, `INTERCITY_TRAIN`, `INTER_REGIONAL_TRAIN`, `REGIONAL_TRAIN`, `CITY_TRAIN`, `SUBWAY`, `TRAM`, `BUS`, `FERRY`, `FLIGHT`, `CAR`, `TAXI`, `SHUTTLE`, `BIKE`, `SCOOTER`, `WALK`

Use SQL literals such as `stop_place.schedule_type = 'ARRIVAL'` and `message.message_type = 'DISRUPTION'`.

## Current Snapshot Surface

Snapshot entities under `Navigator.Data/Entities/Statistics`:

- `DatabaseSizeSnapshot`: database-size trend.
- `RisIdSnapshot`: active/inactive RIS ID trend.
- `JourneySnapshot`: recorded journey trend.

Keyless view entities under `Navigator.Data/Entities/Views`:

- `HourlyStationSnapshot`: hourly station and transport grain. Supports arrival/departure counts, cancellations, delay sums, delay sample counts, punctual counts, delay severity buckets, platform changes, additional stops, demand stops, and no-passenger-change stops.
- `DailyJourneyServiceSnapshot`: daily transport, journey type, and operator grain. Supports journey counts, journey cancellations, replacement transport counts, stop counts, delay sums/samples, message counts, and disruption message counts.
- `DailyStationMessageSnapshot`: daily station, transport, message type, and normalized message fields grain. Supports distinct message counts, affected stop-place counts, and affected journey counts.

Materialized views in the `statistics` schema should have unique indexes matching their grain so `REFRESH MATERIALIZED VIEW CONCURRENTLY` remains available. Existing refresh jobs use `pg_cron`; stagger jobs when multiple dashboard snapshots refresh on the same cadence.

## Metric Design Guidance

Prefer materialized views that store additive facts, then compute ratios or averages later. Good additive columns include:

- `*_count`
- `*_cancellation_count`
- `*_delay_sum`
- `*_delay_sample_count`
- `*_punctual_count`
- `*_delay_minor_count`
- `*_delay_major_count`
- `*_delay_severe_count`
- `*_platform_change_count`
- `*_additional_count`
- `*_demand_count`
- `*_no_passenger_change_count`
- `*_message_count`
- `*_disruption_count`

Delay metrics:

- `delay` is generated as seconds from `actual_time - planned_time`.
- Exclude cancelled stop places from delay sums, delay averages, punctuality rates, and delay buckets by default.
- Keep a valid denominator, preferably explicit sample counts such as `arrival_delay_sample_count` and `departure_delay_sample_count`.
- Preserve negative delays unless the user asks for lateness-only metrics.
- A common punctual threshold is `delay <= 359` seconds, matching "up to 5:59 minutes late".
- Useful severity buckets are minor `360..899` seconds, major `900..3599` seconds, and severe `>= 3600` seconds.

Cancellation metrics:

- Journey cancellation lives on `core.journeys.cancelled`.
- Stop-place cancellation lives on `core.journey_stop_places.cancelled`.
- Use stop-place cancellation for station arrival/departure transparency.
- Use journey cancellation for journey-level, operator-level, and transport-level reliability.

Platform metrics:

- Platform change can be derived when both `planned_platform` and `actual_platform` are non-null and different.
- Keep arrival and departure platform changes separate for station operations.
- Show platform-change rates with counts and total non-cancelled stop samples available in tooltip/details.

Transport metrics:

- Use `journey_transports.transport_type` as the primary transport dimension.
- Keep `replacement_transport_type` available for replacement-service metrics.
- Use `category`, `category_internal`, `label`, `line`, and `number` for drilldowns. Avoid these high-cardinality fields in broad global materialized-view grains unless the dashboard explicitly needs them.

Operator and journey-type metrics:

- Join `core.journeys` to `core.journey_administrations` through `administration_id`.
- Use `operator_code` for stable grouping and `operator_name` for display.
- Include `journey_type` when dashboards compare regular service with replacement, relief, or extra service.
- Prefer ranked bars for operator counts/rates and daily trend lines for changes over time.

Message and disruption metrics:

- Use `core.journey_messages` for journey-level message counts by `message_type`, `code`, `disruption_cause`, `disruption_effect`, or `note_category`.
- Use `core.journey_stop_place_messages` only when attributing messages to stations or stop places.
- Beware bridge-table fanout: count distinct `(message.id, message.date)` when joining messages through stop-place associations.
- Prefer Pareto bars or ranked tables for disruption causes; use daily stacked bars when the question is about message-type trend.

## Materialized View Shapes

Station-hour operational view:

- Grain: `bucket_hour`, `eva_number`, `transport_type`
- Measures: arrivals, departures, cancellations, delay sums, delay sample counts, punctual counts, delay buckets, platform changes, additional/demand/no-passenger-change counts
- Best for: general hourly trends, station summaries, selected-station timelines, transport filters, station maps

Journey-service daily view:

- Grain: `bucket_day`, `transport_type`, `journey_type`, `operator_code`, `operator_name`
- Measures: journeys, journey cancellations, replacement transports, stop counts, delay sums/samples, message counts, disruption counts
- Best for: operator reliability, journey type share, replacement service, operator trend tables

Station-message daily view:

- Grain: `bucket_day`, `eva_number`, `transport_type`, `message_type`, normalized code/cause/effect/category fields
- Measures: distinct message count, affected stop-place count, affected journey count
- Best for: station disruption hotspots, disruption cause rankings, message-type trends

Create a new view only when an existing view cannot answer the metric without losing dimensions or doing expensive fanout joins at request time.

## SQL Conventions

Use explicit aliases and filtered aggregates:

```sql
count(*) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL') AS arrival_count,
count(*) FILTER (
  WHERE stop_place.schedule_type = 'ARRIVAL'
    AND stop_place.cancelled IS TRUE
) AS arrival_cancellation_count,
coalesce(sum(stop_place.delay) FILTER (
  WHERE stop_place.schedule_type = 'ARRIVAL'
    AND stop_place.cancelled IS NOT TRUE
), 0) AS arrival_delay_sum
```

Store numerator and denominator for averages/rates:

```sql
coalesce(sum(stop_place.delay) FILTER (...), 0) AS arrival_delay_sum,
count(*) FILTER (...) AS arrival_delay_sample_count
```

Then calculate `delay_sum / delay_sample_count`, `punctual_count / delay_sample_count`, or `cancellation_count / count` in C# or the query layer with zero-denominator handling.

For concurrent refreshes:

```sql
CREATE UNIQUE INDEX UX_some_snapshot
ON statistics.some_snapshot (bucket_hour, eva_number, transport_type);

REFRESH MATERIALIZED VIEW CONCURRENTLY statistics.some_snapshot;
```

The unique index must cover the complete row grain. If a grain includes nullable dimensions, coalesce them in the view or avoid concurrent refresh until uniqueness is guaranteed.

## EF Core And API Integration

When adding or changing a materialized view metric:

1. Add or update a C# view entity under `Navigator.Data/Entities/Views`.
2. Add a `DbSet<T>` to `Navigator.Data/DataContext.cs`.
3. Add an `IEntityTypeConfiguration<T>` under statistics configurations.
4. Configure it as keyless and excluded from normal migrations:

```csharp
builder.ToTable("view_name", "statistics", table => table.ExcludeFromMigrations());
builder.HasNoKey();
builder.ToView("view_name", "statistics");
```

5. Add a raw SQL migration for the materialized view, unique index, and optional `pg_cron` refresh.
6. Update the EF model snapshot after changing mapped entities.
7. Extend `MetricSeriesType`, request models, data point models, API DTOs, JSON derived types, and Mapperly mappings when the API surface changes.
8. Extend `StatisticsRepository.GetMetricAsync` by dispatching the request type to a typed private query method.
9. Regenerate/validate frontend API client types after DTO or enum changes.

Existing generic request families:

- Snapshot requests: database size, RIS IDs, total journeys.
- `TransportTypeDistributionMetricRequest`: transport distribution.
- `HourlyTransportSnapshotMetricRequest`: general hourly transport/station aggregates, optionally stepped and transport-filtered.
- `StationSummaryMetricRequest`: station-level summaries over a range, optionally EVA-filtered.
- `StationTimeSeriesMetricRequest`: selected-station time series by EVA and transport.
- `JourneyServiceMetricRequest`: daily journey/operator/service metrics with transport, journey type, and operator filters.
- `MessageSummaryMetricRequest`: disruption/message metrics with transport, message type, EVA, and limit filters.

Repository defaults:

- Default empty transport filters to `Enum.GetValues<TransportType>()`.
- Default empty journey/message type filters to all relevant enum values, except disruption-specific metrics can default to `MessageType.Disruption`.
- Use `AsNoTracking()` for read-only metric queries.
- Return `MetricUnit.Count` for counts, `MetricUnit.Seconds` for delays, `MetricUnit.Bytes` for size, and `MetricUnit.Percent` only when values are already percent-scaled.
- Keep zero denominators explicit: return `0` only when the product decision accepts it, otherwise expose sample counts so the UI can mark no-data cases.

## Operational Notes

- `Navigator.Preflight` applies EF migrations before station bootstrap. If a run should apply migrations only, use the existing skip-station-bootstrap option rather than triggering station discovery.
- Temporary local connection-string edits should be restored after migration/preflight work.
- For dashboard transparency, no UI chart should show a rate without making numerator and denominator available through tooltip, details, paired series, or a table.
