import { t } from "elysia";

const ProductOccurrenceInMeasurementSchema = t.Object({
	date: t.String({ format: "date", description: "Date of the measurement" }),
	totalStops: t.Number({ description: "Total number of stops on this date" }),
	totalUniqueJourneys: t.Number({ description: "Total number of unique journeys on this date" }),
	products: t.Array(t.Object({
		productType: t.String({ description: "Type of the product" }),
		stopCount: t.Number({ description: "Total number of stops for this product on this date" }),
		uniqueJourneys: t.Number({ description: "Total number of unique journeys for this product on this date" })
	}))
});

const ProductOccurrenceSchema = t.Object({
	totalStops: t.Number({ description: "Total number of stops" }),
	totalUniqueJourneys: t.Number({ description: "Total number of unique journeys" }),
	measurements: t.Array(ProductOccurrenceInMeasurementSchema)
});

const SingleProductInGeneralMeasurementSchema = t.Object({
	productType: t.String({ description: "Type of the product" }),
	total: t.Number({ description: "Total value for this product type on this date" })
})

const GeneralMeasurementSchema = t.Object({
	date: t.String({ format: "date", description: "Date of the measurement" }),
	total: t.Number({ description: "Total value for this date" }),
	products: t.Array(SingleProductInGeneralMeasurementSchema)
});

const GeneralDelayStatisticSchema = t.Object({
	average: t.Number({ description: "Average delay in seconds" }),
	minimum: t.Number({ description: "Minimum delay in seconds" }),
	maximum: t.Number({ description: "Maximum delay in seconds" }),
});

const SingleProductDelayStatisticSchema = t.Intersect([
	GeneralDelayStatisticSchema,
	t.Object({
		productType: t.String({ description: "Type of the product" })
	})
]);

const DelayMeasurementStatisticSchema = t.Intersect([
	GeneralDelayStatisticSchema,
	t.Object({
		date: t.String({ format: "date", description: "Date of the measurement" }),
		products: t.Array(SingleProductDelayStatisticSchema)
	})
]);

const TimeStatisticsSchema = t.Object({
	total: t.Object({
		total: t.Number({ description: "Total number of arrivals/ departures for this date" }),
		measurements: t.Array(GeneralMeasurementSchema),
	}),
	delay: t.Intersect([
		GeneralDelayStatisticSchema,
		t.Object({
			measurements: t.Array(DelayMeasurementStatisticSchema)
		})
	]),
	tooEarly: t.Object({
		total: t.Number({ description: "Total number of arrivals/ departures that were too early for this date" }),
		measurements: t.Array(GeneralMeasurementSchema)
	}),
	punctual: t.Object({
		total: t.Number({ description: "Total number of punctual arrivals/ departures for this date" }),
		measurements: t.Array(GeneralMeasurementSchema)
	}),
	delayed: t.Object({
		total: t.Number({ description: "Total number of delayed arrivals/ departures for this date" }),
		measurements: t.Array(GeneralMeasurementSchema)
	}),
	platformChanges: t.Object({
		total: t.Number({ description: "Total number of arrivals/ departures with platform changes for this date" }),
		measurements: t.Array(GeneralMeasurementSchema)
	})
});

const CancellationSchema = t.Object({
	total: t.Number({ description: "Total number of cancellations" }),
	measurements: t.Array(GeneralMeasurementSchema)
});

const JourneyStatisticsSchema = t.Object({
	executionTime: t.Number({ description: "Specifies the time in ms how long the analysis took" }),
	products: ProductOccurrenceSchema,
	arrival: TimeStatisticsSchema,
	departure: TimeStatisticsSchema,
	cancellations: CancellationSchema
});

export { ProductOccurrenceInMeasurementSchema, ProductOccurrenceSchema, SingleProductInGeneralMeasurementSchema, GeneralMeasurementSchema, SingleProductDelayStatisticSchema, GeneralDelayStatisticSchema, DelayMeasurementStatisticSchema, TimeStatisticsSchema, CancellationSchema, JourneyStatisticsSchema };