# Statistics Metrics Plan

This document maps the current `POST /api/v1/statistics/metrics` surfaces to the global `/statistics` page and the future station-specific `/statistics/:evaNumber` page. Message metrics are intentionally out of scope.

## Current Global `/statistics`

The global page uses one shared scope: time range, arrival/departure, transport types, and replacement-transport inclusion. The first render is loaded through SSR; later scope or metric changes are loaded by the components.

Visible metric areas:

- Station Quality Map: `STATION_EVENT_QUALITY_SUMMARY` without `evaNumbers`, rendered as either density or value-colored station points.
- Network Quality Trends: `NETWORK_STATION_EVENT_QUALITY_TIME_SERIES`, grouped by hour and transport type.
- Administration Ranking: `ADMINISTRATION_RANKING` across the whole journey-route quality view.
- Line Ranking: `LINE_RANKING` across the whole journey-route quality view, with local journey-description and number filters.

Supported station-event series:

- `STATION_EVENT_COUNT`
- `STATION_EVENT_CANCELLATION_COUNT`
- `STATION_EVENT_CANCELLATION_RATE`
- `STATION_EVENT_DELAY_AVERAGE`
- `STATION_EVENT_PUNCTUALITY5_RATE`
- `STATION_EVENT_PUNCTUALITY15_RATE`

Supported ranking series:

- `ADMINISTRATION_RANKING_COUNT`
- `ADMINISTRATION_RANKING_CANCELLATION_COUNT`
- `ADMINISTRATION_RANKING_CANCELLATION_RATE`
- `ADMINISTRATION_RANKING_AVERAGE_DELAY`
- `ADMINISTRATION_RANKING_PUNCTUALITY5_RATE`
- `ADMINISTRATION_RANKING_PUNCTUALITY15_RATE`
- `LINE_RANKING_COUNT`
- `LINE_RANKING_CANCELLATION_COUNT`
- `LINE_RANKING_CANCELLATION_RATE`
- `LINE_RANKING_AVERAGE_DELAY`
- `LINE_RANKING_PUNCTUALITY5_RATE`
- `LINE_RANKING_PUNCTUALITY15_RATE`

## Additional Global Candidates

These can be added with the existing materialized views and request types:

- Network KPI strip: event count, cancellation rate, average delay, punctuality 5, punctuality 15 for the applied scope.
- Transport-type breakdown: stacked or grouped bars from `NETWORK_STATION_EVENT_QUALITY_TIME_SERIES`.
- Station ranking lists: top/bottom stations from `STATION_EVENT_QUALITY_SUMMARY`, useful beside or below the map.
- Cancellation volume trend: `STATION_EVENT_CANCELLATION_COUNT` as an additional network trend, separate from cancellation rate.

Project/system metrics should stay outside the `/statistics` network dashboard unless a dedicated operations view is added:

- `DATABASE_SIZE_SNAPSHOT`
- `RIS_ID_SNAPSHOT`
- `JOURNEY_SNAPSHOT`

## Future `/statistics/:evaNumber`

Station pages should use the selected EVA number as scope and should not render the global map.

Recommended station views:

- Station KPI strip from `STATION_EVENT_QUALITY_SUMMARY` with `evaNumbers: [evaNumber]`.
- Station quality trends from `STATION_EVENT_QUALITY_TIME_SERIES` with `evaNumbers: [evaNumber]`.
- Operator ranking from `ADMINISTRATION_RANKING` with `evaNumbers: [evaNumber]`.
- Line ranking from `LINE_RANKING` with `evaNumbers: [evaNumber]`.

The station page should keep the same shared scope model as `/statistics`, with station-specific ranking filters remaining local to their widgets.

## Interaction Defaults

- Default time range: last 7 days.
- Default schedule type: `DEPARTURE`.
- Default transport filter: all transport types, represented by omitting `transportTypes`.
- Default replacement-transport setting: included.
- Shared scope changes are draft-based and require Apply; metric-specific controls reload only their own widget.
