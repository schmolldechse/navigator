WITH date_range AS (
    SELECT generate_series(:start_date::date, :end_date::date, '1 day'::interval)::date AS date
),
included_station_transports AS (
    SELECT DISTINCT transport_name
    FROM core.station_transports
    WHERE (
        :includedEvaNumbers::integer[] IS NULL
        OR COALESCE(array_length(:includedEvaNumbers::integer[], 1), 0) = 0
        OR eva_number = ANY (:includedEvaNumbers::integer[])
    )
    AND (
        NOT :onlyIncludeActiveTransports::bool
        OR querying_enabled IS TRUE
    )
    AND (
        :includedTransportTypes::varchar[] IS NULL
        OR COALESCE(array_length(:includedTransportTypes::varchar[], 1), 0) = 0
        OR transport_name ILIKE ANY (:includedTransportTypes::varchar[])
    )
),
date_transport_combinstion AS (
    SELECT date_range.date, included_station_transports.transport_name
    FROM date_range
    CROSS JOIN included_station_transports
)
SELECT
    combination.date AS date,
    journey_transports.type AS transport_type,
    --- Arrival Specific Details
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'ARRIVAL'), 0) AS arrival_count,
    COALESCE(SUM (CASE
        WHEN :punishCancellations::bool AND via_stop.cancelled::bool
             THEN :delayThreshold::integer
        ELSE via_stop.delay
        END) FILTER (WHERE via_stop.type = 'ARRIVAL'), 0) AS arrival_delay_sum,
    COALESCE(AVG (CASE
        WHEN :punishCancellations::bool AND via_stop.cancelled::bool
            THEN :delayThreshold::integer
        ELSE via_stop.delay
        END) FILTER (WHERE via_stop.type = 'ARRIVAL'), 0) AS arrival_delay_avg,
    COALESCE(MIN(via_stop.delay) FILTER (WHERE via_stop.type = 'ARRIVAL'), 0) AS arrival_delay_min,
    COALESCE(MAX(CASE
        WHEN :punishCancellations::bool AND via_stop.cancelled::bool
            THEN :delayThreshold::integer
        ELSE via_stop.delay
        END) FILTER (WHERE via_stop.type = 'ARRIVAL'), 0) AS arrival_delay_max,
    COALESCE(SUM(1) FILTER (WHERE via_stop.cancelled::bool IS FALSE AND via_stop.type = 'ARRIVAL' AND via_stop.delay < 0), 0) AS arrival_too_early_count,
    COALESCE(SUM(1) FILTER (WHERE via_stop.cancelled::bool IS FALSE AND via_stop.type = 'ARRIVAL' AND via_stop.delay >= 0 AND via_stop.delay <= :delayThreshold::integer), 0) AS arrival_punctual_count,
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'ARRIVAL' AND (via_stop.delay > :delayThreshold::integer OR (via_stop.cancelled::bool AND :punishCancellations::bool))), 0) AS arrival_delayed_count,
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'ARRIVAL' AND via_stop.planned_platform <> via_stop.actual_platform), 0) AS arrival_platform_changes,
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'ARRIVAL' AND via_stop.cancelled), 0) AS arrival_cancellation_count,
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'ARRIVAL' AND via_stop.additional), 0) AS arrival_additional_count,
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'ARRIVAL' AND via_stop.demand), 0) AS arrival_demand_count,
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'ARRIVAL' AND via_stop.no_passenger_change), 0) AS arrival_no_exchange_possible_count,
    --- Departure Specific Details
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'DEPARTURE'), 0) AS departure_count,
    COALESCE(SUM (CASE
        WHEN :punishCancellations::bool AND via_stop.cancelled::bool
            THEN :delayThreshold::integer
        ELSE via_stop.delay
        END) FILTER (WHERE via_stop.type = 'DEPARTURE'), 0) AS departure_delay_sum,
    COALESCE(AVG (CASE
        WHEN :punishCancellations::bool AND via_stop.cancelled::bool
            THEN :delayThreshold::integer
        ELSE via_stop.delay
        END) FILTER (WHERE via_stop.type = 'DEPARTURE'), 0) AS departure_delay_avg,
    COALESCE(MIN(via_stop.delay) FILTER (WHERE via_stop.type = 'DEPARTURE'), 0) AS departure_delay_min,
    COALESCE(MAX(CASE
        WHEN :punishCancellations::bool AND via_stop.cancelled::bool
            THEN :delayThreshold::integer
        ELSE via_stop.delay
        END) FILTER (WHERE via_stop.type = 'DEPARTURE'), 0) AS departure_delay_max,
    COALESCE(SUM(1) FILTER (WHERE via_stop.cancelled::bool IS FALSE AND via_stop.type = 'DEPARTURE' AND via_stop.delay < 0), 0) AS departure_too_early_count,
    COALESCE(SUM(1) FILTER (WHERE via_stop.cancelled::bool IS FALSE AND via_stop.type = 'DEPARTURE' AND via_stop.delay >= 0 AND via_stop.delay <= :delayThreshold::integer), 0) AS departure_punctual_count,
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'DEPARTURE' AND (via_stop.delay > :delayThreshold::integer OR (via_stop.cancelled::bool AND :punishCancellations::bool))), 0) AS departure_delayed_count,
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'DEPARTURE' AND via_stop.planned_platform <> via_stop.actual_platform), 0) AS departure_platform_changes,
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'DEPARTURE' AND via_stop.cancelled), 0) AS departure_cancellation_count,
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'DEPARTURE' AND via_stop.additional), 0) AS departure_additional_count,
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'DEPARTURE' AND via_stop.demand), 0) AS departure_demand_count,
    COALESCE(SUM(1) FILTER (WHERE via_stop.type = 'DEPARTURE' AND via_stop.no_passenger_change), 0) AS departure_no_exchange_possible_count
FROM date_transport_combinstion combination
LEFT JOIN core.journeys ON combination.date = journeys.date
LEFT JOIN core.journey_transports ON journeys.journey_id = journey_transports.journey_id AND combination.transport_name = journey_transports.type
    AND (
        :includeJourneysByName::varchar IS NULL
        OR :includeJourneysByName::varchar = ''
        OR journey_transports.category ~ :includeJourneysByName::varchar
        OR journey_transports.journey_description ~ :includeJourneysByName::varchar
        OR (journey_transports.category || ' ' || journey_transports.line) ~ :includeJourneysByName::varchar
    )
    AND (
        :includeJourneysByNumber::integer[] IS NULL
        OR COALESCE(array_length(:includeJourneysByNumber::integer[], 1), 0) = 0
        OR journey_transports.number = ANY (:includeJourneysByNumber::integer[])
    )
LEFT JOIN core.journey_scheduled_stop_places AS via_stop ON journeys.journey_id = via_stop.journey_id
    AND (
        :includedEvaNumbers::integer[] IS NULL
        OR COALESCE(array_length(:includedEvaNumbers::integer[], 1), 0) = 0
        OR via_stop.station_eva_number = ANY (:includedEvaNumbers::integer[])
    )
WHERE journey_transports.type IS NOT NULL AND journey_transports.type <> ''
GROUP BY combination.date, journey_transports.type
ORDER BY combination.date;