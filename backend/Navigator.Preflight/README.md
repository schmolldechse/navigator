# Navigator.Preflight

`Navigator.Preflight` is the database bootstrap utility for Navigator. It applies EF Core migrations and can seed the station catalog from Deutsche Bahn station sources before the API and daemon start using the database.

The station discovery and merge approach is based on the ideas used by [derhuerst/db-hafas-stations](https://github.com/derhuerst/db-hafas-stations), which builds a broad station/stop dataset from `RIS::Stations` and StaDa. Navigator reimplements this flow for its own .NET/TimescaleDB bootstrap process instead of consuming the npm package directly.

## Responsibilities

- Apply pending EF Core migrations.
- Discover stop places through `RIS::Stations`.
- Load station metadata from StaDa.
- Merge RIS and StaDa station records into Navigator's internal station model.
- Store EVA numbers, names, coordinates, RIL100 identifiers, transport types, query flags, and station weights.

## Data Sources

Preflight uses:

- [RIS::Stations](https://developer-docs.deutschebahn.com/doku/apis/ris-stations-10686906) for stop-place discovery by geographic position.
- [StaDa](https://developers.deutschebahn.com/db-api-marketplace/apis/product) for station metadata such as RIL100 identifiers and price categories.
- [DB API Marketplace](https://developers.deutschebahn.com/db-api-marketplace/apis/product) for API product access and credentials.

## Implementation Reference

The discovery shape follows the public `db-hafas-stations` approach:

- fetch a broad set of stop places from `RIS::Stations`
- fetch station metadata from StaDa
- merge records by EVA number and related identifiers
- preserve station names, coordinates, transport coverage, and railway identifiers for local lookup

`db-hafas-stations` publishes its package code under the ISC license and documents separate data-license terms for the generated station data. Navigator's repository is licensed under Apache-2.0, while any data retrieved from Deutsche Bahn APIs remains subject to the respective upstream terms.

The repository-level attribution is recorded in [`THIRD-PARTY-NOTICES.md`](../../THIRD-PARTY-NOTICES.md).

## Configuration

Required configuration:

```text
ConnectionStrings:DefaultConnection
STATIONS_CLIENT_ID
STATIONS_API_KEY
```

Station bootstrapping can be skipped while still applying migrations:

```text
Preflight:SkipStationBootstrap=true
```

## Discovery Process

The discovery process recursively scans a broad bounding box, writes temporary RIS and StaDa response files, and then merges them into station records. Temporary discovery files are stored below the system temp directory:

```text
navigator/station_discovery
```

A `manifest.json` file marks a completed discovery run. Remove the temporary discovery directory if a full re-discovery is required.

## Development

Run from the repository root:

```sh
dotnet run --project backend/Navigator.Preflight
```

## References

- [Navigator.Data README](../Navigator.Data/README.md)
- [DB API Marketplace](https://developers.deutschebahn.com/db-api-marketplace/apis/product)
- [DB API Marketplace Terms of Use](https://developers.deutschebahn.com/db-api-marketplace/apis/nutzungsbedingungen)
- [RIS::Stations documentation](https://developer-docs.deutschebahn.com/doku/apis/ris-stations-10686906)
- [derhuerst/db-hafas-stations](https://github.com/derhuerst/db-hafas-stations)
- [Navigator third-party notices](../../THIRD-PARTY-NOTICES.md)
