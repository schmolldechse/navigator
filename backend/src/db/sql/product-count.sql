WITH date_range AS (
    -- CTE 1 : Range of dates with 1-day intervals
    SELECT generate_series(:start_date::date, :end_date::date, '1 day'::interval)::date AS date
),
product_types AS (
    -- CTE 2 : Distinct product types by filtering journeys, product and the date range
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
product_count AS (
    -- CTE 4 : Count of stops and unique journeys
    -- Filters by:
    -- - evaNumber_filter (ex. ARRAY[8000001, 8000002])
    SELECT
        journeys.journey_date AS date,
        journeys.product_type AS product_type,
        COUNT(via_stops.journey_id)::integer AS stop_count,
        COUNT(DISTINCT journeys.journey_id)::integer AS unique_journeys
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
    combinations.date,
    combinations.product_type,
    COALESCE(product_count.stop_count, 0)::integer AS stop_count,
    COALESCE(product_count.unique_journeys, 0)::integer AS unique_journeys
FROM date_product_combinations combinations
LEFT JOIN product_count ON (combinations.date = product_count.date) AND (combinations.product_type = product_count.product_type)
ORDER BY combinations.date, combinations.product_type;