# Navigator.Data

This repository contains the core data layer for the **Navigator Backend**. It serves as the central library for all Database Entities and HTTP API Repositories, facilitating communication with internal databases and external third-party services.

## Journey Analytics Storage

Journey data is stored in two layers:

1. **Raw journey tables** in the `core` schema keep the complete normalized source data:
   - `core.journeys`
   - `core.journey_transports`
   - `core.journey_stop_places`
   - `core.journey_messages`
   - `core.journey_stop_place_messages`

2. **Analytics fact tables** in the `statistics` schema keep pre-shaped rows for TimescaleDB continuous aggregates:
   - `statistics.journey_event_quality_facts`
   - `statistics.journey_route_quality_facts`

The fact tables are intentionally derived data. They do not replace the raw journey tables and must not be treated as the source of truth.

### Why Fact Tables Exist

The previous PostgreSQL materialized views could run complex SQL directly over the normalized journey tables because a refresh recomputed the whole view. TimescaleDB continuous aggregates work differently: they are designed to incrementally refresh time buckets from a hypertable. They work best when the aggregate query is a straightforward `time_bucket(...)` plus `GROUP BY` over a single fact source.

The historical quality views need more than simple aggregation:

- joins between journeys, transports, and stop places
- origin and destination station resolution
- terminal delay selection per journey
- replacement transport detection

That shaping is therefore done once when journeys are imported, and again during historical backfill. Continuous aggregates then only count and sum already-prepared facts.

### Current Data Flow

```text
RIS Journey API
  -> EF Journey graph
  -> core raw journey hypertables
  -> statistics fact hypertables
  -> TimescaleDB continuous aggregates
  -> API metric builders
```

`statistics.journey_event_quality_facts` contains one row per stop-place event and powers:

- `statistics.station_line_route_quality_hourly`

`statistics.journey_route_quality_facts` contains one row per journey and powers:

- `statistics.journey_route_quality_hourly`

The API continues to read from the hourly statistics views. The raw journey tables remain available for audits, reprocessing, and future analytics.

### TimescaleDB Refresh and Compression Windows

Journey imports can write data several months in the past because RIS IDs may be discovered late in a timetable period or continued across operating dates. TimescaleDB policies therefore keep the active refresh window wider than a half-year timetable period:

- continuous aggregate refresh window: `210 days`
- compression policy for raw journey and fact hypertables: `240 days`

This keeps the current timetable period plus buffer uncompressed and refreshable. Compression starts only after the data is expected to be historically stable.

## Model Generation Guide

When adding or updating models for the Deutsche Bahn third-party APIs (RIS), strictly follow the guide below.

1. **Obtain NSwag Tool**: Download and install the NSwag.ConsoleCore package from [NuGet](https://www.nuget.org/packages/NSwag.ConsoleCore).

2. **Go to the DB API Marketplace**: Navigate to the **"RIS-API"** category in the dropdown menu.
   ![DB API Marketplace](../docs/model-generation-1.png)

3. **Choose specific API**: Select the relevant API product (e.g. `RIS::Boards`, `RIS::Journeys`, `RIS::Stations`, etc.). Scroll down to the **Zugehörige APIs** (Associated APIs) section and click on the specific API you need.
   ![API Selection](../docs/model-generation-2.png)

4. **Download OpenAPI Specification**: Scroll down to the bottom of the page and use one of the download option to download the specification file (usually `.json` or `.yaml`).
   ![Download API Specification](../docs/model-generation-3.png)

5. **Generate Models using NSwag**: Use the NSwag.ConsoleCore CLI downloaded before to generate the C# client models from the downloaded specification. Run the following command, replacing `<path of the spec>` and `<output file>` with your specific paths:

   ```bash
   nswag openapi2csclient /input:<path of spec> /output:<output file>.cs /namespace:Navigator.Data /JsonLibrary:SystemTextJson
   ```

6. **Integration**: Take the generated model file and place it in the `Models/Ris` directory.

---
