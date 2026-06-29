# Navigator.Daemon

`Navigator.Daemon` is the background worker host for Navigator. It uses Quartz jobs to discover RIS IDs, import completed journeys, write project snapshots, deactivate stale IDs, and project journey analytics facts.

## Responsibilities

- Query monitored station boards through `RIS::Boards` to discover active journey IDs.
- Fetch completed operating days from `RIS::Journeys`.
- Store raw journeys through `Navigator.Data`.
- Write database-size, RIS-ID, and journey-count snapshots.
- Project raw journey data into analytics fact tables for TimescaleDB continuous aggregates.
- Re-check and deactivate RIS IDs that no longer appear after a stale window.

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
