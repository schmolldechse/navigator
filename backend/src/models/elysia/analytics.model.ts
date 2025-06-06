import { t } from "elysia";

const MeasurementByDateSchema = t.Object({
	date: t.String({ format: "date" }),
	totalMeasured: t.Number()
});

const CompleteMeasurementSchema = t.Object({
	totalMeasured: t.Number(),
	measurements: t.Array(MeasurementByDateSchema)
});

const DelaySchema = t.Object({
	average: t.Number({ description: "Specifies the average delay in seconds" }),
	minimum: t.Number({ description: "Specifies the minimum delay in seconds" }),
	maximum: t.Number({ description: "Specifies the maximum delay in seconds" })
});

const DelayByDateMeasurementSchema = t.Intersect([
	MeasurementByDateSchema,
	DelaySchema
]);

const TimeStatisticsSchema = t.Object({
	total: CompleteMeasurementSchema,
	delay: t.Intersect([
		DelaySchema,
		t.Object({
			measurements: t.Array(DelayByDateMeasurementSchema)
		})
	]),
	tooEarly: CompleteMeasurementSchema,
	punctual: CompleteMeasurementSchema,
	delayed: CompleteMeasurementSchema,
	platformChanges: CompleteMeasurementSchema
});

const StopAnalyticsSchema = t.Object({
	executionTime: t.Optional(t.Number({ description: "Specifies the time in ms how long the analysis took" })),
	relatedEvaNumbers: t.Array(t.Number(), {
		description: "An array of related evaNumbers which where considered in the analysis"
	}),
	totalJourneys: t.Number({ description: "Total number of journeys considered in the analysis" }),
	products: t.Array(
		t.Intersect([
			CompleteMeasurementSchema,
			t.Object({ type: t.String({ description: "Type of the product" }) })
		])
	),
	arrival: TimeStatisticsSchema,
	departure: TimeStatisticsSchema,
	cancellations: CompleteMeasurementSchema
});

export { MeasurementByDateSchema, CompleteMeasurementSchema, TimeStatisticsSchema, DelayByDateMeasurementSchema, StopAnalyticsSchema };
