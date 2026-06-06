# Navigator

Navigator is an independent full-stack project for collecting, storing, and visualizing German public transport operating data. It turns station boards and journey records into inspectable statistics for punctuality, delays, cancellations, stations, operators, and lines.

> [!IMPORTANT]
> Navigator is an independent open-source project and is not affiliated with Deutsche Bahn AG, DB InfraGO, DB Regio, DB Fernverkehr, or any other transport company.

## What Navigator Does

- Tracks discovered RIS IDs from monitored station arrivals and departures.
- Imports journey details with stops, planned times, forecast or actual times, cancellations, platforms, operators, and route information.
- Builds global and station-specific quality metrics for event counts, cancellations, average delay, and 5-minute / 15-minute punctuality.
- Compares operators and lines through ranking views.
- Shows station quality geographically with a map-based metric view.
- Exposes a .NET API with OpenAPI/Scalar documentation and a SvelteKit dashboard frontend.

Navigator is a growing measurement system, not an official or complete railway archive. Its statistics describe the data collected by this project from the stations and transport types that are enabled for gathering.

## Data Sources

Navigator combines several Deutsche Bahn data interfaces:

- [DB API Marketplace](https://developers.deutschebahn.com/db-api-marketplace/apis/product) for API product access and credentials.
- [RIS::Stations](https://developer-docs.deutschebahn.com/doku/apis/ris-stations-10686906) and [StaDa](https://developers.deutschebahn.com/db-api-marketplace/apis/product) for station discovery, stop-place metadata, EVA numbers, RIL100 identifiers, and transport coverage.
- [RIS::Boards](https://developer-docs.deutschebahn.com/doku/apis/ris-boards-10686900) for station arrivals and departures. These boards are used to discover active journey IDs.
- [RIS::Journeys](https://developer-docs.deutschebahn.com/doku/apis/ris-journeys-10582266) for detailed journey records after a journey ID has been discovered.
- Deutsche Bahn's Vendo location search endpoint for station search in the public API and frontend.

Data obtained through Deutsche Bahn APIs remains subject to the terms of the respective API products and the [DB API Marketplace Terms of Use](https://developers.deutschebahn.com/db-api-marketplace/apis/nutzungsbedingungen).

Deutsche Bahn commonly reports operational punctuality using a 5:59-minute threshold. Navigator exposes 5-minute and 15-minute quality metrics for its own collected dataset, but these values should not be read as official Deutsche Bahn punctuality figures.

## Similar Projects

- [piebro/deutsche-bahn-data](https://github.com/piebro/deutsche-bahn-data): a related project focused on historical Deutsche Bahn data as downloadable datasets. Navigator borrows the principle of transparent data provenance, but its primary interface is the API and dashboard backed by a TimescaleDB database.
- [derhuerst/db-hafas-stations](https://github.com/derhuerst/db-hafas-stations): a station dataset/build pipeline using RIS::Stations and StaDa. Navigator's preflight station bootstrap is based on the same idea of discovering and merging these station sources for local use.

## Architecture

Navigator is split into focused backend services and a Svelte frontend:

- [`frontend`](frontend/README.md): SvelteKit/Svelte 5 dashboard served with Bun and backed by generated OpenAPI types.
- [`backend/Navigator.Api`](backend/Navigator.Api/README.md): ASP.NET Core API for stations, timetables, journeys, and statistics.
- [`backend/Navigator.Daemon`](backend/Navigator.Daemon/README.md): Quartz-based background worker for discovery, imports, snapshots, and analytics projection.
- [`backend/Navigator.Preflight`](backend/Navigator.Preflight/README.md): database bootstrap utility that runs migrations and seeds station data.
- [`backend/Navigator.Data`](backend/Navigator.Data/README.md): Entity Framework Core data layer, repository implementations, generated DB API models, and TimescaleDB-backed statistics models.
- [`backend/Navigator.Observability`](backend/Navigator.Observability/README.md): shared logging, tracing, and correlation helpers.

The main data flow is:

```text
RIS::Stations / StaDa
  -> station bootstrap
  -> monitored station boards
  -> RIS ID catalog
  -> RIS::Journeys batch import
  -> raw journey tables
  -> analytics fact tables
  -> TimescaleDB continuous aggregates
  -> statistics API
  -> dashboard
```

Journey data is stored in raw normalized tables first. Derived fact tables and TimescaleDB continuous aggregates are used for fast statistics, but they are not treated as the source of truth.

## Local Development

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Bun](https://bun.sh/)
- Docker or another PostgreSQL/TimescaleDB setup
- Deutsche Bahn API Marketplace credentials for the required products:
  - `RIS::Stations`: `STATIONS_CLIENT_ID`, `STATIONS_API_KEY`
  - `RIS::Boards`: `BOARDS_CLIENT_ID`, `BOARDS_API_KEY`
  - `RIS::Journeys`: `JOURNEYS_CLIENT_ID`, `JOURNEYS_API_KEY`

### Start The Database

```sh
docker compose -f docker/navigator-database/docker-compose.yaml up -d
```

The default local connection string expects:

```text
Host=localhost;Port=5432;Database=navigator;Username=navigator;Password=password
```

### Bootstrap And Run

Run preflight after configuring the `STATIONS_*` credentials:

```sh
dotnet run --project backend/Navigator.Preflight
```

Run the API after configuring the `BOARDS_*` and `JOURNEYS_*` credentials:

```sh
dotnet run --project backend/Navigator.Api
```

Run the daemon when data collection or analytics projection should be active:

```sh
dotnet run --project backend/Navigator.Daemon
```

Start the frontend:

```sh
cd frontend
bun install
cp .env.example .env
bun run dev
```

`frontend/.env.example` sets:

```text
PUBLIC_API_URL=http://localhost:5019/
```

## Development Commands

Backend:

```sh
cd backend
dotnet format backend.slnx --verify-no-changes
dotnet build backend.slnx
```

Frontend:

```sh
cd frontend
bun run lint
bun run check
bun run build
```

Regenerate frontend API types while the API is running:

```sh
cd frontend
bun run generate:api
```

## Notes On Coverage

Navigator does not start from a complete timetable archive. It discovers journeys by querying enabled station boards, extracting journey IDs, and then requesting detailed records for completed operating days. Coverage therefore depends on the monitored stations, enabled transport types, DB API availability, and the time period during which Navigator has been collecting data.

The timetable preview route exists in the frontend, but the current dashboard focus is the statistics experience.

## License

Navigator is licensed under the [Apache License 2.0](LICENSE).

The project license covers the code in this repository. Data obtained through Deutsche Bahn APIs remains subject to the respective upstream API and data terms.

Third-party references and attributions are recorded in [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).

## References

- [DB API Marketplace](https://developers.deutschebahn.com/db-api-marketplace/apis/product)
- [DB API Marketplace Terms of Use](https://developers.deutschebahn.com/db-api-marketplace/apis/nutzungsbedingungen)
- [RIS::Boards documentation](https://developer-docs.deutschebahn.com/doku/apis/ris-boards-10686900)
- [RIS::Journeys documentation](https://developer-docs.deutschebahn.com/doku/apis/ris-journeys-10582266)
- [RIS::Stations documentation](https://developer-docs.deutschebahn.com/doku/apis/ris-stations-10686906)
- [derhuerst/db-hafas-stations](https://github.com/derhuerst/db-hafas-stations)
- [Third-party notices](THIRD-PARTY-NOTICES.md)
- [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0)
