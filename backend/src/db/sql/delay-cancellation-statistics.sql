WITH date_range AS (
    -- CTE 1 : Range of dates with 1-day intervals
SELECT generate_series(:start_date::date, :end_date::date, '1 day'::interval)::date AS date
),
product_types AS (
    -- CTE 2 : Distinct product types for the date range
    -- Filters by:
    -- - product_filter (ex. ARRAY['Sbahnen'])
    -- - journeyName_filter (REGEX pattern, ex. 'MEX.*')
    -- - journeyNumber_filter (REGEX pattern, ex. '^12345$')
    SELECT DISTINCT product_type
    FROM core.journeys
    WHERE journey_date BETWEEN :start_date::date AND :end_date::date
    AND (
        :product_filter::varchar[] IS NULL
        OR coalesce(array_length(:product_filter::varchar[], 1), 0) = 0
        OR product_type = ANY(:product_filter::varchar[])
    )
    AND (
        :journeyName_filter::varchar IS NULL
        OR :journeyName_filter = ''
        OR journeys.journey_name ~ :journeyName_filter
    )
    AND (
        :journeyNumber_filter::varchar IS NULL
        OR :journeyNumber_filter = ''
        OR journeys.journey_number ~ :journeyNumber_filter
    )
),
date_product_combinations AS (
    -- CTE 3 : Cartesian product of dates and product types
    SELECT
        range.date,
        products.product_type
    FROM date_range range
    CROSS JOIN product_types products
),
stats_per_product AS (
    -- CTE 4 : Core statistics
    -- Filters by:
    -- - evaNumber_filter (ex. ARRAY[8000001, 8000002])
    SELECT
        journeys.journey_date AS date,
        journeys.product_type AS product_type,
        -- Arrival statistics
        SUM(CASE
            WHEN via_stops.arrival_planned_time IS NOT NULL
            AND via_stops.arrival_actual_time IS NOT NULL
            THEN 1
            ELSE 0
        END) AS arrival_count,
        SUM(via_stops.arrival_delay) AS arrival_delay_sum,
        AVG(via_stops.arrival_delay) AS arrival_delay_avg,
        MIN(via_stops.arrival_delay) AS arrival_delay_min,
        MAX(via_stops.arrival_delay) AS arrival_delay_max,
        SUM(CASE
            WHEN via_stops.arrival_actual_platform IS NOT NULL
            AND via_stops.arrival_planned_platform IS NOT NULL
            AND via_stops.arrival_actual_platform <> via_stops.arrival_planned_platform
            THEN 1
            ELSE 0
        END) AS arrival_platform_changes,
        SUM(CASE
            WHEN via_stops.arrival_delay < 0
            THEN 1
            ELSE 0
        END) AS arrival_too_early_count,
        SUM(CASE
            WHEN via_stops.arrival_delay >= 0 AND via_stops.arrival_delay <= :delay_threshold::integer
            THEN 1
            ELSE 0
        END) AS arrival_punctual_count,
        SUM(CASE
            WHEN via_stops.arrival_delay > :delay_threshold::integer
            THEN 1
            ELSE 0
        END) AS arrival_delayed_count,
        -- Departure
        SUM(CASE
            WHEN via_stops.departure_planned_time IS NOT NULL
            AND via_stops.departure_actual_time IS NOT NULL
            THEN 1
            ELSE 0
        END) AS departure_count,
        SUM(via_stops.departure_delay) AS departure_delay_sum,
        AVG(via_stops.departure_delay) AS departure_delay_avg,
        MIN(via_stops.departure_delay) AS departure_delay_min,
        MAX(via_stops.departure_delay) AS departure_delay_max,
        SUM(CASE
            WHEN via_stops.departure_actual_platform IS NOT NULL
            AND via_stops.departure_planned_platform IS NOT NULL
            AND via_stops.departure_actual_platform <> via_stops.departure_planned_platform
            THEN 1
            ELSE 0
        END) AS departure_platform_changes,
        SUM(CASE
            WHEN via_stops.departure_delay < 0
            THEN 1
            ELSE 0
        END) AS departure_too_early_count,
        SUM(CASE
            WHEN via_stops.departure_delay >= 0 AND via_stops.departure_delay <= :delay_threshold::integer
            THEN 1
            ELSE 0
        END) AS departure_punctual_count,
        SUM(CASE
            WHEN via_stops.departure_delay > :delay_threshold::integer
            THEN 1
            ELSE 0
        END) AS departure_delayed_count,
        -- Core
        SUM(CASE
            WHEN via_stops.cancelled IS TRUE
            THEN 1
            ELSE 0
        END) AS cancellations,
    FROM core.journeys AS journeys
    JOIN core."journey_via-stops" AS via_stops ON journeys.journey_date = via_stops.journey_date
    AND journeys.journey_id = via_stops.journey_id
    WHERE journeys.journey_date BETWEEN :start_date::date AND :end_date::date
    AND (
        :evaNumber_filter::integer[] IS NULL
        OR coalesce(array_length(:evaNumber_filter::integer[], 1), 0) = 0
        OR via_stops.eva_number = ANY(:evaNumber_filter::integer[])
    )
    GROUP BY journeys.journey_date, journeys.product_type
)
SELECT
    -- General
    combinations.date,
    combinations.product_type,
    -- Arrival
    COALESCE(stats_per_product.arrival_count, 0)::integer AS arrival_count,
    COALESCE(stats_per_product.arrival_delay_sum, 0)::integer AS arrival_delay_sum,
    COALESCE(stats_per_product.arrival_delay_avg, 0)::double precision AS arrival_delay_avg,
    COALESCE(stats_per_product.arrival_delay_min, 0)::double precision AS arrival_delay_min,
    COALESCE(stats_per_product.arrival_delay_max, 0)::double precision AS arrival_delay_max,
    COALESCE(stats_per_product.arrival_platform_changes, 0)::integer AS arrival_platform_changes,
    COALESCE(stats_per_product.arrival_too_early_count, 0)::integer AS arrival_too_early_count,
    COALESCE(stats_per_product.arrival_punctual_count, 0)::integer AS arrival_punctual_count,
    COALESCE(stats_per_product.arrival_delayed_count, 0)::integer AS arrival_delayed_count,
    -- Departure
    COALESCE(stats_per_product.departure_count, 0)::integer AS departure_count,
    COALESCE(stats_per_product.departure_delay_sum, 0)::integer AS departure_delay_sum,
    COALESCE(stats_per_product.departure_delay_avg, 0)::double precision AS departure_delay_avg,
    COALESCE(stats_per_product.departure_delay_min, 0)::double precision AS departure_delay_min,
    COALESCE(stats_per_product.departure_delay_max, 0)::double precision AS departure_delay_max,
    COALESCE(stats_per_product.departure_platform_changes, 0)::integer AS departure_platform_changes,
    COALESCE(stats_per_product.departure_too_early_count, 0)::integer AS departure_too_early_count,
    COALESCE(stats_per_product.departure_punctual_count, 0)::integer AS departure_punctual_count,
    COALESCE(stats_per_product.departure_delayed_count, 0)::integer AS departure_delayed_count,
    -- Core Statistics
    COALESCE(stats_per_product.cancellations, 0)::integer AS cancellations,
FROM date_product_combinations combinations
LEFT JOIN stats_per_product ON (combinations.date = stats_per_product.date) AND (combinations.product_type = stats_per_product.product_type)
ORDER BY combinations.date, combinations.product_type;
