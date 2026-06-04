# Navigator.Daemon

This project runs the scheduled background jobs that discover stations, collect RIS IDs, import journey details, and write operational snapshots for Navigator.

### Timetable Change Window

Germany follows the coordinated European rail timetable change rhythm:

- the main annual timetable change starts after the second Saturday in December
- the smaller mid-year timetable change starts after the second Saturday in June

In code this is represented as the following Sunday at `00:00 UTC`. The job derives relevant timetable changes from that rule instead of keeping a hardcoded date list.

For new RIS IDs, the first journey lookup starts seven days before the last timetable change that is visible from the latest completed operating date. This allows late-discovered IDs to backfill the current timetable period without reaching into still-changing live data.

Journey details are intentionally fetched only for operating days that are expected to be complete. The daemon uses `UTC today - 2 days` as the latest fetchable operating date because an in-progress journey can otherwise expose stale intermediate RIS data that cannot be reliably detected afterward.
