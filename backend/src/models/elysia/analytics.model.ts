import { t } from "elysia";

const MeasurementSchema = t.Object({
	date: t.String({ format: "date" }),
	amount: t.Number()
});

const StopAnalyticsSchema = t.Object({
	executionTime: t.Optional(t.Number({ description: "Specifies the time in ms how long the analysis took" })),
	relatedEvaNumbers: t.Array(t.Number(), {
		description: "An array of related evaNumbers which where considered in the analysis"
	}),
	products: t.Array(
		t.Object({
			type: t.String({ description: "Type of product" }),
			amount: t.Number(),
			measurements: t.Array(MeasurementSchema)
		})
	),
	arrival: t.Object({
		total: t.Number({ description: "Specifies the total number of arrivals" }),
		averageDelay: t.Number({ description: "Specifies the average delay in seconds" }),
		minimumDelay: t.Number({ description: "Specifies the minimum delay in seconds" }),
		maximumDelay: t.Number({ description: "Specifies the maximum delay in seconds" }),
		totalTooEarly: t.Number({ description: "Specifies the total number of arrivals which were too early" }),
		totalPunctual: t.Number({ description: "Specifies the total number of arrivals which were punctual" }),
		totalDelayed: t.Number({ description: "Specifies the total number of arrivals which were delayed" }),
		totalPlatformChanges: t.Number({ description: "Specifies the total number of arrivals which had a platform change" })
	}),
	departure: t.Object({
		total: t.Number({ description: "Specifies the total number of departures" }),
		averageDelay: t.Number({ description: "Specifies the average delay in seconds" }),
		minimumDelay: t.Number({ description: "Specifies the minimum delay in seconds" }),
		maximumDelay: t.Number({ description: "Specifies the maximum delay in seconds" }),
		totalTooEarly: t.Number({ description: "Specifies the total number of departures which were too early" }),
		totalPunctual: t.Number({ description: "Specifies the total number of departures which were punctual" }),
		totalDelayed: t.Number({ description: "Specifies the total number of departures which were delayed" }),
		totalPlatformChanges: t.Number({ description: "Specifies the total number of departures which had a platform change" })
	}),
	cancellations: t.Number({ description: "Specifies the total number of cancellations" })
});

export { MeasurementSchema, StopAnalyticsSchema };
