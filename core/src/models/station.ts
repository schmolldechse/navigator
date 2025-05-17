import { t } from "elysia";
import { TimeSchema } from "./time";
import { MessageSchema } from "./message";

const StationSchema = t.Object({
	name: t.String({ description: "Name of a station" }),
	evaNumber: t.Number({ description: "Unique evaNumber of a station" }),
	coordinates: t.Object({
		latitude: t.Number({ description: "Latitude of the station" }),
		longitude: t.Number({ description: "Longitude of the station" })
	}, { description: "Coordinates of the station" }),
	ril100: t.Array(t.String(), { description: "List of RIL100 numbers of the station" }),
	products: t.Array(t.String(), { description: "List of products available at the station" })
});
type Station = typeof StationSchema.static;

export { StationSchema, type Station };

const StopSchema = t.Intersect([
	StationSchema,
	t.Object({
		cancelled: t.Boolean({ description: "Marks if the stop is cancelled" }),
		additional: t.Optional(t.Boolean({ description: "Marks if the stop is additional" })),
		separation: t.Optional(t.Boolean({ description: "Specifies if there is an separation of vehicles at this station" })),
		nameParts: t.Optional(t.Array(t.Object({
			type: t.String(),
			value: t.String()
		}), { description: "List of name parts" })),
		departure: t.Optional(TimeSchema),
		arrival: t.Optional(TimeSchema),
		messages: t.Optional(t.Array(MessageSchema))
	})
]);
type Stop = typeof StopSchema.static;

export { StopSchema, type Stop };

const StopAnalyticsSchema = t.Object({
	executionTime: t.Optional(t.Number({ description: "Specifies the time in ms how long the analysis took" })),
	relatedEvaNumbers: t.Array(t.Number(), { description: "An array of related evaNumbers which where considered in the analysis" }),
	foundByQuery: t.Optional(t.Number({ description: "Amount of connections found in the database" })),
	parsingSucceeded: t.Number({ description: "Amount of connections which could be parsed" }),
	parsingFailed: t.Number({ description: "Amount of connections which could not be parsed" }),
	products: t.Record(t.String(), t.Number(), {
		examples: [{ "Busse": 1, "NahverkehrsonstigeZuege": 2 }],
		description: "Specifies how often a product has occurred"
	}),
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
type StopAnalytics = typeof StopAnalyticsSchema.static;

export { StopAnalyticsSchema, type StopAnalytics };