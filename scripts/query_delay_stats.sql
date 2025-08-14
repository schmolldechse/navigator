WITH date_range AS (
    SELECT generate_series(:start_date::date, :end_date::date, '1 day'::interval)::date AS date
),
allowed_products AS (
    SELECT DISTINCT name AS product_type
    FROM core.station_products
    WHERE (
        :evaNumbers::integer[] IS NULL
        OR COALESCE(array_length(:evaNumbers::integer[], 1), 0) = 0
        OR eva_number = ANY (:evaNumbers::integer[])
    )
    AND (
        NOT :onlyActiveProducts::bool
        OR querying_enabled IS TRUE
    )
    AND (
        :productTypes::varchar[] IS NULL
        OR COALESCE(array_length(:productTypes::varchar[], 1), 0) = 0
        OR name ILIKE ANY (:productTypes::varchar[])
    )
),
date_product_combinations AS (
    SELECT date_range.date, allowed_products.product_type
    FROM date_range
    CROSS JOIN allowed_products
)
SELECT
    combination.date AS date,
    journeys.product_type AS product_type,
    --- Arrival Count
    COALESCE(SUM(1) FILTER (WHERE via_stops.arrival_planned_time IS NOT NULL AND via_stops.arrival_actual_time IS NOT NULL), 0) AS arrival_count,
    --- Arrival Delay
    COALESCE(SUM (CASE
        WHEN :punishCancelled::bool AND via_stops.cancelled::bool
            THEN :delayThreshold::integer
        ELSE via_stops.arrival_delay
        END) FILTER (WHERE via_stops.arrival_planned_time IS NOT NULL AND via_stops.arrival_actual_time IS NOT NULL), 0) AS arrival_delay_sum,
    COALESCE(AVG(CASE
        WHEN :punishCancelled::bool AND via_stops.cancelled::bool
            THEN :delayThreshold::integer
        ELSE via_stops.arrival_delay
        END) FILTER (WHERE via_stops.arrival_planned_time IS NOT NULL AND via_stops.arrival_actual_time IS NOT NULL), 0) AS arrival_delay_avg,
    COALESCE(MIN (via_stops.arrival_delay) FILTER (WHERE via_stops.arrival_planned_time IS NOT NULL AND via_stops.arrival_actual_time IS NOT NULL), 0) AS arrival_delay_min,
    COALESCE(MAX (CASE
        WHEN :punishCancelled::bool AND via_stops.cancelled::bool
            THEN :delayThreshold::integer
        ELSE via_stops.arrival_delay
        END) FILTER (WHERE via_stops.arrival_planned_time IS NOT NULL AND via_stops.arrival_actual_time IS NOT NULL), 0) AS arrival_delay_max,
    --- Arrival Platform Changes
    COALESCE(SUM (1) FILTER (WHERE via_stops.arrival_actual_platform <> via_stops.arrival_planned_platform), 0) AS arrival_platform_changes,
    --- Arrival Too Early, Punctual, Too Delayed Counts
    COALESCE(SUM (1) FILTER (WHERE via_stops.cancelled::bool IS FALSE AND via_stops.arrival_delay < 0), 0) AS arrival_too_early_count,
    COALESCE(SUM (1) FILTER (WHERE via_stops.cancelled::bool IS FALSE AND via_stops.arrival_delay >= 0 AND via_stops.arrival_delay <= :delayThreshold::integer), 0) AS arrival_punctual_count,
    COALESCE(SUM (1) FILTER (WHERE via_stops.arrival_delay > :delayThreshold::integer OR (:punishCancelled::bool AND via_stops.cancelled::bool AND via_stops.arrival_planned_time IS NOT NULL AND arrival_actual_time IS NOT NULL)), 0) AS arrival_too_delayed_count,
    --- Departure Count
    COALESCE(SUM(1) FILTER (WHERE via_stops.departure_planned_time IS NOT NULL AND via_stops.departure_actual_time IS NOT NULL), 0) AS departure_count,
    --- Departure Delay
    COALESCE(SUM (CASE
        WHEN :punishCancelled::bool AND via_stops.cancelled::bool
            THEN :delayThreshold::integer
        ELSE via_stops.departure_delay
        END) FILTER (WHERE via_stops.departure_planned_time IS NOT NULL AND via_stops.departure_actual_time IS NOT NULL), 0) AS departure_delay_sum,
    COALESCE(AVG(CASE
        WHEN :punishCancelled::bool AND via_stops.cancelled::bool
            THEN :delayThreshold::integer
        ELSE via_stops.departure_delay
        END) FILTER (WHERE via_stops.departure_planned_time IS NOT NULL AND via_stops.departure_actual_time IS NOT NULL), 0) AS departure_delay_avg,
    COALESCE(MIN (via_stops.departure_delay) FILTER (WHERE via_stops.departure_planned_time IS NOT NULL AND via_stops.departure_actual_time IS NOT NULL), 0) AS departure_delay_min,
    COALESCE(MAX (CASE
        WHEN :punishCancelled::bool AND via_stops.cancelled::bool
            THEN :delayThreshold::integer
        ELSE via_stops.departure_delay
        END) FILTER (WHERE via_stops.departure_planned_time IS NOT NULL AND via_stops.departure_actual_time IS NOT NULL), 0) AS departure_delay_max,
    --- Departure Platform Changes
    COALESCE(SUM (1) FILTER (WHERE via_stops.departure_actual_platform <> via_stops.departure_planned_platform), 0) AS departure_platform_changes,
    --- Departure Too Early, Punctual, Too Delayed Counts
    COALESCE(SUM (1) FILTER (WHERE via_stops.cancelled::bool IS FALSE AND via_stops.departure_delay < 0), 0) AS departure_too_early_count,
    COALESCE(SUM (1) FILTER (WHERE via_stops.cancelled::bool IS FALSE AND via_stops.departure_delay >= 0 AND via_stops.departure_delay <= :delayThreshold::integer), 0) AS departure_punctual_count,
    COALESCE(SUM (1) FILTER (WHERE via_stops.departure_delay > :delayThreshold::integer OR (:punishCancelled::bool AND via_stops.cancelled::bool AND via_stops.departure_planned_time IS NOT NULL AND departure_actual_time IS NOT NULL)), 0) AS departure_too_delayed_count,
    -- Cancellations
    COALESCE(SUM(1) FILTER (WHERE via_stops.cancelled::bool), 0) AS cancellations
FROM date_product_combinations combination
LEFT JOIN core.journeys ON combination.date = journeys.journey_date AND combination.product_type ILIKE journeys.product_type
    AND (
        :journeyName::varchar IS NULL
        OR :journeyName::varchar = ''
        OR journeys.journey_name ~ :journeyName::varchar
    )
    AND (
        :journeyNumber::varchar IS NULL
        OR :journeyNumber::varchar = ''
        OR journeys.journey_number ~ :journeyNumber::varchar
    )
LEFT JOIN core."journey_via-stops" AS via_stops ON journeys.journey_id = via_stops.journey_id AND journeys.journey_date = via_stops.journey_date
    AND (
        :evaNumbers::integer[] IS NULL
        OR COALESCE(array_length(:evaNumbers::integer[], 1), 0) = 0
        OR via_stops.eva_number = ANY (:evaNumbers::integer[])
    )
WHERE journeys.product_type IS NOT NULL AND journeys.product_type <> ''
GROUP BY combination.date, journeys.product_type
ORDER BY combination.date;