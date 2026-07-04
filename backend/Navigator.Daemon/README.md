# Navigator.Daemon

`Navigator.Daemon` is the background worker host for Navigator. It uses Quartz jobs to discover RIS IDs, import completed journeys, write project snapshots, deactivate stale IDs, and refresh journey statistics aggregates.

## Responsibilities

- Query monitored station boards through `RIS::Boards` to discover active journey IDs.
- Fetch completed operating days from `RIS::Journeys`.
- Store raw journeys through `Navigator.Data`.
- Write database-size, RIS-ID, and journey-count snapshots.
- Refresh bounded journey statistics aggregate windows and dependent TimescaleDB continuous aggregates.
- Re-check and deactivate RIS IDs that no longer appear after a stale window.

## Statistics Refresh

`StatisticsAggregateRefreshJob` is the continuous target-system updater. It does not use a journey backlog and does not copy row-level facts. Every run plans a bounded set of UTC windows, recomputes those windows from `core`, writes aggregate rollups, then explicitly refreshes every dependent statistics CAGG for the coalesced recomputed ranges.

The job uses the same unified recompute path as `Navigator.StatisticsRecovery`: `statistics.recompute_quality_hourly_rollups` materializes one temporary window workset and derives both event and journey rollups from it. The daemon only applies this to bounded recent or catch-up windows selected by `StatisticsRefreshWindowPlanner`; it does not run the historical recovery range and it does not call the legacy journey fact projection path.

`StatisticsRefresh:HotLookbackHours` controls the recent windows refreshed each run. `StatisticsRefresh:CatchupLookbackHours` controls the horizon for failed and rotating late-data windows, but the job never recomputes the whole catch-up horizon in one scheduled execution. `StatisticsRefresh:MaxWindowsPerRun` caps work per trigger. The 1-hour end offset avoids refreshing the currently moving hour.

## Timetable Change Window

Germany follows the coordinated European rail timetable change rhythm:

- the main annual timetable change starts after the second Saturday in December
- the smaller mid-year timetable change starts after the second Saturday in June

Navigator represents this as the following Sunday at `00:00 UTC`. For new RIS IDs, the first journey lookup starts seven days before the last visible timetable change from the latest completed operating date.

Journey details are intentionally fetched only for operating days expected to be complete. The daemon uses `UTC today - 2 days` as the latest fetchable operating date because an in-progress journey can expose stale intermediate RIS data that cannot be reliably detected afterward.

## Configuration

The daemon requires:

```text
ConnectionStrings:DefaultConnection
BOARDS_CLIENT_ID
BOARDS_API_KEY
JOURNEYS_CLIENT_ID
JOURNEYS_API_KEY
```

Optional observability settings are shared with the API:

```text
Observability:Otlp:Enabled
Observability:Otlp:Endpoint
```

Do not commit real credentials or production connection strings.

## Development

Run from the repository root:

```sh
dotnet run --project backend/Navigator.Daemon
```

## References

- [Navigator.Data README](../Navigator.Data/README.md)
- [Navigator.Observability README](../Navigator.Observability/README.md)
- [Quartz.NET documentation](https://www.quartz-scheduler.net/documentation/)
- [RIS::Boards documentation](https://developer-docs.deutschebahn.com/doku/apis/ris-boards-10686900)
- [RIS::Journeys documentation](https://developer-docs.deutschebahn.com/doku/apis/ris-journeys-10582266)
