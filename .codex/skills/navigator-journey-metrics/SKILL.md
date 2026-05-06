---
name: navigator-journey-metrics
description: Design Navigator journey dashboard metrics from the PostgreSQL/EF Core journey schema. Use when Codex needs to create or revise journey, station, delay, cancellation, transport, disruption, operator, or materialized-view analytics for Navigator; add EF Core keyless view entities for statistics views; or extend StatisticsRepository metric support.
---

# Navigator Journey Metrics

## Overview

Use this skill to design dashboard metrics and PostgreSQL materialized views for Navigator journey analytics. Prefer the existing snapshot pattern: aggregate expensive journey facts in PostgreSQL, expose the result through keyless EF Core view entities, and keep repository logic focused on filtering, grouping, rates, and response shaping.

## Workflow

1. Read `references/journey-metrics.md` before proposing schema, SQL, EF Core models, API DTOs, or repository changes.
2. Identify the dashboard question first: count, rate, sum, average, distribution, top-N, station summary, operator summary, disruption/message analysis, or trend over time.
3. Choose the lowest-grain snapshot/view that can answer the question without losing needed dimensions. Common dimensions are time bucket, EVA station, transport type, schedule type, journey type, operator, cancellation state, and message type.
4. Preserve partition-friendly joins: join journey tables by both `journey_id` and `date`, and filter by date/time ranges as early as the metric allows.
5. For delay metrics, use stored `core.journey_stop_places.delay` in seconds. Exclude cancelled stop places from delay sums and averages unless the user explicitly wants cancellation-inclusive calculations.
6. For rates, expose or fetch the raw numerator and denominator so dashboards can show transparent tooltips and avoid hiding sparse data.
7. For PostgreSQL materialized views that should refresh concurrently, create a unique index that covers the complete materialized-view grain.
8. For EF Core integration, map materialized views as keyless view entities under `Navigator.Data/Entities/Views`, add a `DbSet`, and configure `ToTable(..., table => table.ExcludeFromMigrations())`, `HasNoKey()`, and `ToView(...)`.

## Metric Defaults

- Use `statistics` schema for dashboard snapshots/views and `core` schema for raw journey facts.
- Use hourly buckets for station/transport operations and daily buckets for operator, journey-service, and disruption summaries unless the request needs a different grain.
- Keep raw counts and sums in the view; compute averages/rates from numerator and denominator in repository/service code when practical.
- Use PostgreSQL enum literals such as `'ARRIVAL'`, `'DEPARTURE'`, `'DISRUPTION'`, and `'REGULAR'` in SQL.
- Prefer additive columns in materialized views: counts, cancellation counts, delay sums, valid-delay counts, punctual counts, delay buckets, platform-change counts, and message counts.

## References

- `references/journey-metrics.md`: Navigator journey schema, existing materialized view pattern, metric ideas, SQL conventions, and EF Core integration notes.
