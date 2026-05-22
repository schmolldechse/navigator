# Statistics Metrics Plan

This plan describes the metric surfaces available from `POST /api/v1/statistics/metrics` and how they should map to the global statistics page and future station-specific pages.

## Global `/statistics`

The first implemented global metric is the station metric map. It uses `STATION_EVENT_QUALITY_SUMMARY` and intentionally leaves `evaNumbers` unset, so the backend returns one value per station for the selected network slice.

Supported station metric map series:

- `STATION_EVENT_COUNT`
- `STATION_EVENT_CANCELLATION_COUNT`
- `STATION_EVENT_CANCELLATION_RATE`
- `STATION_EVENT_DELAY_AVERAGE`
- `STATION_EVENT_PUNCTUALITY5_RATE`
- `STATION_EVENT_PUNCTUALITY15_RATE`

The global page can later add network-wide time series with `NETWORK_STATION_EVENT_QUALITY_TIME_SERIES`. These should use the same station-event series, `scheduleType`, time range, and optional `transportTypes`, but no `evaNumbers`.

Ranking metrics that fit the global page:

- `ADMINISTRATION_RANKING_*` for operator/administration ranking across the whole network.
- `LINE_RANKING_*` for line ranking across the whole network.

System/project metrics that can stay outside the station metric map experience:

- `DATABASE_SIZE_SNAPSHOT`
- `RIS_ID_SNAPSHOT`
- `JOURNEY_SNAPSHOT`

## Station-Specific `/statistics/:evaNumber`

Station-specific pages should not render a global station metric map. They should use the selected station EVA number as the scope for station quality metrics and rankings.

Recommended station quality metrics:

- `STATION_EVENT_QUALITY_SUMMARY` with `evaNumbers: [evaNumber]` for station-level KPI summaries.
- `STATION_EVENT_QUALITY_TIME_SERIES` with `evaNumbers: [evaNumber]` for charts over time.

Supported station quality series:

- `STATION_EVENT_COUNT`
- `STATION_EVENT_CANCELLATION_COUNT`
- `STATION_EVENT_CANCELLATION_RATE`
- `STATION_EVENT_DELAY_AVERAGE`
- `STATION_EVENT_PUNCTUALITY5_RATE`
- `STATION_EVENT_PUNCTUALITY15_RATE`

Ranking metrics that fit a station page:

- `ADMINISTRATION_RANKING_*` with `evaNumbers: [evaNumber]` to show operators ranked by quality at that station.
- `LINE_RANKING_*` with `evaNumbers: [evaNumber]` to show lines ranked by quality at that station.

## Interaction Defaults

- Default time range: last 7 days.
- Default schedule type: `DEPARTURE`.
- Default metric: `STATION_EVENT_COUNT`.
- Default transport filter: all transport types, represented by omitting `transportTypes`.
- The station metric map is global only; station-specific pages use charts, KPI summaries, and rankings instead.
