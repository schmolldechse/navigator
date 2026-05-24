using Microsoft.EntityFrameworkCore;
using Navigator.Data.Enums;
using Navigator.Data.Enums.Metric;
using Navigator.Data.Models.Statistics;
using Navigator.Data.Models.Statistics.DataPoint;
using Navigator.Data.Models.Statistics.Request;
using Navigator.Data.Models.Statistics.Subject;
using Npgsql;
using NpgsqlTypes;
using System.Text.RegularExpressions;

namespace Navigator.Data.Repository.StatisticsRepository.MetricSeriesBuilders;

public sealed class LineRankingMetricSeriesBuilder(
    DataContext dataContext
) : MetricSeriesBuilder<LineRankingMetricRequest>
{
    private static readonly IReadOnlyDictionary<MetricSeriesType, MetricDefinition> Definitions =
        new Dictionary<MetricSeriesType, MetricDefinition>
        {
            [MetricSeriesType.LineRankingCount] = new(
                MetricUnit.Count,
                query => query.OrderByDescending(row => row.Count),
                row => row.Count),
            [MetricSeriesType.LineRankingCancellationCount] = new(
                MetricUnit.Count,
                query => query.OrderByDescending(row => row.CancellationCount),
                row => row.CancellationCount),
            [MetricSeriesType.LineRankingCancellationRate] = new(
                MetricUnit.Percent,
                query => query.OrderByDescending(row => row.Count == 0 ? 0 : 100m * row.CancellationCount / row.Count),
                row => row.Count == 0 ? 0 : 100m * row.CancellationCount / row.Count,
                row => new() { Numerator = row.CancellationCount, Denominator = row.Count }),
            [MetricSeriesType.LineRankingAverageDelay] = new(
                MetricUnit.Seconds,
                query => query.OrderByDescending(row => row.DelaySampleCount == 0 ? 0 : (decimal)row.DelaySumSeconds / row.DelaySampleCount),
                row => row.DelaySampleCount == 0 ? 0 : (decimal)row.DelaySumSeconds / row.DelaySampleCount,
                row => new() { Numerator = row.DelaySumSeconds, Denominator = row.DelaySampleCount }),
            [MetricSeriesType.LineRankingPunctuality5Rate] = new(
                MetricUnit.Percent,
                query => query.OrderByDescending(row => row.DelaySampleCount == 0 ? 0 : 100m * row.Punctual5Count / row.DelaySampleCount),
                row => row.DelaySampleCount == 0 ? 0 : 100m * row.Punctual5Count / row.DelaySampleCount,
                row => new() { Numerator = row.Punctual5Count, Denominator = row.DelaySampleCount }),
            [MetricSeriesType.LineRankingPunctuality15Rate] = new(
                MetricUnit.Percent,
                query => query.OrderByDescending(row => row.DelaySampleCount == 0 ? 0 : 100m * row.Punctual15Count / row.DelaySampleCount),
                row => row.DelaySampleCount == 0 ? 0 : 100m * row.Punctual15Count / row.DelaySampleCount,
                row => new() { Numerator = row.Punctual15Count, Denominator = row.DelaySampleCount })
        };

    protected override async Task<MetricSeries> BuildAsync(LineRankingMetricRequest request)
    {
        var definition = GetDefinition(request.SeriesType);
        var limit = Math.Clamp(request.Limit, 1, 500);
        var offset = Math.Max(request.Offset, 0);

        var ranking = request.EvaNumbers.Any()
            ? BuildStationLineRankingQuery(request)
            : BuildGlobalLineRankingQuery(request);

        var totalItems = await ranking.CountAsync();
        var rows = await definition.Order(ranking)
            .ThenByDescending(row => row.Count)
            .ThenBy(row => row.JourneyDescription)
            .ThenBy(row => row.Number)
            .ThenBy(row => row.TransportType)
            .ThenBy(row => row.AdministrationId)
            .ThenBy(row => row.OriginEvaNumber)
            .ThenBy(row => row.DestinationEvaNumber)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var administrations = await LoadAdministrationsAsync(rows.Select(row => row.AdministrationId));
        var stations = await LoadStationsAsync(rows.SelectMany(row => new[] { row.OriginEvaNumber, row.DestinationEvaNumber }));

        var dataPoints = rows
            .Where(row => administrations.ContainsKey(row.AdministrationId))
            .Select(row => new LineRankingDataPoint
            {
                Line = new LineMetricSubject
                {
                    Number = row.Number,
                    JourneyDescription = row.JourneyDescription,
                    TransportType = row.TransportType
                },
                Administration = administrations[row.AdministrationId],
                StartStation = GetStationSubject(stations, row.OriginEvaNumber),
                EndStation = GetStationSubject(stations, row.DestinationEvaNumber),
                Value = definition.GetValue(row),
                Sample = definition.CreateSample?.Invoke(row)
            })
            .ToList();

        return new()
        {
            SeriesType = request.SeriesType,
            Unit = definition.Unit,
            DataPoints = dataPoints,
            Page = MetricPage.Create(offset, limit, totalItems)
        };
    }

    private IQueryable<LineRankingRow> BuildGlobalLineRankingQuery(LineRankingMetricRequest request)
    {
        var start = request.Start.UtcDateTime;
        var end = request.End.UtcDateTime;
        var transportTypes = StationEventQualityMetricDefinitions.NormalizeTransportTypes(request.TransportTypes);

        var query = dataContext.JourneyRouteQualities
            .AsNoTracking()
            .Where(summary => summary.BucketHour >= start && summary.BucketHour < end)
            .Where(summary => transportTypes.Contains(summary.TransportType))
            .Where(summary => request.IncludeReplacementTransport || !summary.IsReplacementTransport);

        var journeyDescriptionRegex = request.JourneyDescription;
        if (!string.IsNullOrWhiteSpace(journeyDescriptionRegex))
            query = query.Where(summary => Regex.IsMatch(summary.JourneyDescription, journeyDescriptionRegex));
        if (!string.IsNullOrWhiteSpace(request.Number))
            query = query.Where(summary => Regex.IsMatch(summary.Number.ToString(), request.Number));

        return query
            .GroupBy(summary => new
            {
                summary.Number,
                summary.JourneyDescription,
                summary.TransportType,
                summary.AdministrationId,
                summary.OriginEvaNumber,
                summary.DestinationEvaNumber
            })
            .Select(group => new LineRankingRow
            {
                Number = group.Key.Number,
                JourneyDescription = group.Key.JourneyDescription,
                TransportType = group.Key.TransportType,
                AdministrationId = group.Key.AdministrationId,
                OriginEvaNumber = group.Key.OriginEvaNumber,
                DestinationEvaNumber = group.Key.DestinationEvaNumber,
                Count = group.Sum(summary => summary.JourneyCount),
                CancellationCount = group.Sum(summary => summary.JourneyCancelledCount),
                DelaySampleCount = group.Sum(summary => summary.DelaySampleCount),
                DelaySumSeconds = group.Sum(summary => summary.DelaySumSeconds),
                Punctual5Count = group.Sum(summary => summary.Punctual5Count),
                Punctual15Count = group.Sum(summary => summary.Punctual15Count)
            });
    }

    private IQueryable<LineRankingRow> BuildStationLineRankingQuery(LineRankingMetricRequest request)
    {
        const string sql = """
            WITH station_event_candidates AS (
                SELECT
                    stop_place.id,
                    stop_place.journey_id,
                    stop_place.date,
                    stop_place.station_eva_number,
                    stop_place.schedule_type,
                    stop_place.planned_time,
                    transport.number,
                    transport.journey_description,
                    transport.transport_type,
                    journey.administration_id,
                    stop_place.cancelled,
                    stop_place.delay
                FROM core.journey_stop_places AS stop_place
                    JOIN core.journey_transports AS transport
                        ON transport.journey_id = stop_place.journey_id
                            AND transport.date = stop_place.date
                    JOIN core.journeys AS journey
                        ON journey.id = stop_place.journey_id
                            AND journey.date = stop_place.date
                WHERE
                    (
                        cardinality(@eva_numbers) = 0
                        OR stop_place.station_eva_number = ANY(@eva_numbers)
                    )
                    AND stop_place.date BETWEEN @start_date AND @end_date
                    AND stop_place.planned_time >= @start
                    AND stop_place.planned_time < @end
                    AND (
                        @journey_description_regex IS NULL
                        OR transport.journey_description ~ @journey_description_regex
                    )
                    AND (
                        @number_regex IS NULL
                        OR transport.number::text ~ @number_regex
                    )
                    AND transport.transport_type = ANY(@transport_types)
                    AND (
                        @include_replacement_transport
                        OR (
                            journey.journey_type <> 'REPLACEMENT'::core.journey_type
                            AND transport.replacement_transport_type IS NULL
                        )
                    )
            ),
            station_events AS (
                SELECT
                    journey_id,
                    date,
                    number,
                    journey_description,
                    transport_type,
                    administration_id,
                    cancelled,
                    delay
                FROM (
                    SELECT
                        station_event_candidates.*,
                        row_number() OVER (
                            PARTITION BY journey_id, date, station_eva_number
                            ORDER BY
                                CASE WHEN schedule_type = 'DEPARTURE' THEN 0 ELSE 1 END,
                                planned_time DESC,
                                id
                        ) AS event_rank
                    FROM station_event_candidates
                ) AS ranked_station_events
                WHERE event_rank = 1
            ),
            selected_journeys AS (
                SELECT DISTINCT journey_id, date
                FROM station_events
            ),
            routes AS (
                SELECT
                    stop_place.journey_id,
                    stop_place.date,
                    coalesce(
                        (
                            array_agg(
                                stop_place.station_eva_number
                                ORDER BY stop_place.planned_time, stop_place.station_eva_number
                            ) FILTER (WHERE stop_place.schedule_type = 'DEPARTURE')
                        )[1],
                        (
                            array_agg(
                                stop_place.station_eva_number
                                ORDER BY stop_place.planned_time, stop_place.station_eva_number
                            )
                        )[1],
                        0
                    ) AS origin_eva_number,
                    coalesce(
                        (
                            array_agg(
                                stop_place.station_eva_number
                                ORDER BY stop_place.planned_time DESC, stop_place.station_eva_number DESC
                            ) FILTER (WHERE stop_place.schedule_type = 'ARRIVAL')
                        )[1],
                        (
                            array_agg(
                                stop_place.station_eva_number
                                ORDER BY stop_place.planned_time DESC, stop_place.station_eva_number DESC
                            )
                        )[1],
                        0
                    ) AS destination_eva_number
                FROM core.journey_stop_places AS stop_place
                    JOIN selected_journeys
                        ON selected_journeys.journey_id = stop_place.journey_id
                            AND selected_journeys.date = stop_place.date
                GROUP BY
                    stop_place.journey_id,
                    stop_place.date
            )
            SELECT
                station_events.number AS "Number",
                station_events.journey_description AS "JourneyDescription",
                station_events.transport_type AS "TransportType",
                station_events.administration_id AS "AdministrationId",
                routes.origin_eva_number AS "OriginEvaNumber",
                routes.destination_eva_number AS "DestinationEvaNumber",
                count(*)::bigint AS "Count",
                count(*) FILTER (
                    WHERE station_events.cancelled IS TRUE
                )::bigint AS "CancellationCount",
                count(*) FILTER (
                    WHERE station_events.cancelled IS NOT TRUE
                )::bigint AS "DelaySampleCount",
                coalesce(
                    sum(station_events.delay) FILTER (
                        WHERE station_events.cancelled IS NOT TRUE
                    ),
                    0
                )::bigint AS "DelaySumSeconds",
                count(*) FILTER (
                    WHERE station_events.cancelled IS NOT TRUE
                        AND station_events.delay < 300
                )::bigint AS "Punctual5Count",
                count(*) FILTER (
                    WHERE station_events.cancelled IS NOT TRUE
                        AND station_events.delay < 900
                )::bigint AS "Punctual15Count"
            FROM station_events
                JOIN routes
                    ON routes.journey_id = station_events.journey_id
                        AND routes.date = station_events.date
            GROUP BY
                station_events.number,
                station_events.journey_description,
                station_events.transport_type,
                station_events.administration_id,
                routes.origin_eva_number,
                routes.destination_eva_number
            """;

        var parameters = new List<object>
        {
            new NpgsqlParameter("eva_numbers", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = request.EvaNumbers },
            new NpgsqlParameter("start_date", NpgsqlDbType.Date) { Value = DateOnly.FromDateTime(request.Start.UtcDateTime.Date) },
            new NpgsqlParameter("end_date", NpgsqlDbType.Date) { Value = DateOnly.FromDateTime(request.End.UtcDateTime.Date) },
            new NpgsqlParameter("start", NpgsqlDbType.TimestampTz) { Value = request.Start.UtcDateTime },
            new NpgsqlParameter("end", NpgsqlDbType.TimestampTz) { Value = request.End.UtcDateTime },
            new NpgsqlParameter<TransportType[]>("transport_types", StationEventQualityMetricDefinitions.NormalizeTransportTypes(request.TransportTypes)) { DataTypeName = "core.transport_type[]" },
            new NpgsqlParameter("journey_description_regex", NpgsqlDbType.Text) { Value = string.IsNullOrWhiteSpace(request.JourneyDescription) ? DBNull.Value : request.JourneyDescription },
            new NpgsqlParameter("number_regex", NpgsqlDbType.Text) { Value = string.IsNullOrWhiteSpace(request.Number) ? DBNull.Value : request.Number },
            new NpgsqlParameter("include_replacement_transport", NpgsqlDbType.Boolean) { Value = request.IncludeReplacementTransport },
        };

        return dataContext.Database.SqlQueryRaw<LineRankingRow>(sql.ToString(), parameters.ToArray());
    }

    private async Task<Dictionary<Guid, AdministrationMetricSubject>> LoadAdministrationsAsync(IEnumerable<Guid> administrationIds)
    {
        var ids = administrationIds.Distinct().ToArray();
        return await dataContext.Administrations
            .AsNoTracking()
            .Where(administration => ids.Contains(administration.Id))
            .ToDictionaryAsync(
                administration => administration.Id,
                administration => new AdministrationMetricSubject
                {
                    AdministrationId = administration.AdministrationId,
                    OperatorCode = administration.OperatorCode,
                    OperatorName = administration.OperatorName
                });
    }

    private async Task<Dictionary<int, StationMetricSubject>> LoadStationsAsync(IEnumerable<int> evaNumbers)
    {
        var numbers = evaNumbers.Distinct().ToArray();
        return await dataContext.Stations
            .AsNoTracking()
            .Where(station => numbers.Contains(station.EvaNumber))
            .ToDictionaryAsync(
                station => station.EvaNumber,
                station => new StationMetricSubject
                {
                    EvaNumber = station.EvaNumber,
                    Name = station.Name,
                });
    }

    private static StationMetricSubject GetStationSubject(
        IReadOnlyDictionary<int, StationMetricSubject> stations,
        int evaNumber
    ) => stations.TryGetValue(evaNumber, out var station)
        ? station
        : new StationMetricSubject
        {
            EvaNumber = evaNumber
        };

    private static MetricDefinition GetDefinition(MetricSeriesType seriesType) =>
        Definitions.TryGetValue(seriesType, out var definition)
            ? definition
            : throw new NotSupportedException($"Unsupported line ranking metric series type: {seriesType}");

    private sealed record MetricDefinition(
        MetricUnit Unit,
        Func<IQueryable<LineRankingRow>, IOrderedQueryable<LineRankingRow>> Order,
        Func<LineRankingRow, decimal> GetValue,
        Func<LineRankingRow, MetricSample?>? CreateSample = null);

    private sealed class LineRankingRow
    {
        public int Number { get; set; }
        public string JourneyDescription { get; set; } = string.Empty;
        public TransportType TransportType { get; set; }
        public Guid AdministrationId { get; set; }
        public int OriginEvaNumber { get; set; }
        public int DestinationEvaNumber { get; set; }
        public long Count { get; set; }
        public long CancellationCount { get; set; }
        public long DelaySampleCount { get; set; }
        public long DelaySumSeconds { get; set; }
        public long Punctual5Count { get; set; }
        public long Punctual15Count { get; set; }
    }
}
