import { t } from "elysia";

const MeasurementSchema = t.Object({
	date: t.String({ format: "date" }),
	totalMeasured: t.Number()
});

const DelaySchema = t.Object({
	average: t.Number({ description: "Specifies the average delay in seconds" }),
	minimum: t.Number({ description: "Specifies the minimum delay in seconds" }),
	maximum: t.Number({ description: "Specifies the maximum delay in seconds" }),
});

const TimeSchema = t.Object({
	total: t.Object({
		totalMeasured: t.Number(),
		measurements: t.Array(MeasurementSchema)
	}),
	delay: t.Intersect([
		DelaySchema,
		t.Object({
			measurements: t.Array(t.Intersect([
				MeasurementSchema,
				DelaySchema,
			]))
		})
	]),
	tooEarly: t.Object({
		totalMeasured: t.Number(),
		measurements: t.Array(MeasurementSchema)
	}),
	punctual: t.Object({
		totalMeasured: t.Number(),
		measurements: t.Array(MeasurementSchema)
	}),
	delayed: t.Object({
		totalMeasured: t.Number(),
		measurements: t.Array(MeasurementSchema)
	}),
	platformChanges: t.Object({
		totalMeasured: t.Number(),
		measurements: t.Array(MeasurementSchema)
	})
});

const StopAnalyticsSchema = t.Object({
	executionTime: t.Optional(t.Number({ description: "Specifies the time in ms how long the analysis took" })),
	relatedEvaNumbers: t.Array(t.Number(), {
		description: "An array of related evaNumbers which where considered in the analysis"
	}),
	products: t.Array(
		t.Object({
			type: t.String({ description: "Type of product" }),
			totalMeasured: t.Number(),
			measurements: t.Array(MeasurementSchema)
		})
	),
	arrival: TimeSchema,
	departure: TimeSchema,
	cancellations: t.Object({
		totalMeasured: t.Number(),
		measurements: t.Array(MeasurementSchema)
	})
});

export { MeasurementSchema, StopAnalyticsSchema };
