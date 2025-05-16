import type { Time } from "./time.ts";
import type { Message } from "./message.ts";
import { t } from "elysia";

interface Station {
	name: string;
	/**
	 * @isInt evaNumber of a station
	 * @pattern /^\d+$/
	 * @example 8000096
	 */
	evaNumber: number;
	coordinates?: {
		latitude: number;
		longitude: number;
	};
	ril100?: string[];

	/**
	 * contains a list of the values of products {@link Product} of the station
	 */
	products?: string[];
}

interface Stop extends Station {
	cancelled: boolean;
	additional?: boolean;
	separation?: boolean;
	nameParts?: NamePart[];

	departure?: Time;
	arrival?: Time;
	messages?: Message[];
}

const StopAnalyticsSchema = t.Object({
	executionTime: t.Optional(t.Number({ description: "Specifies the time in ms how long the analysis took" })),
	relatedEvaNumbers: t.Array(t.Number(), { description: "An array of related evaNumbers which where considered in the analysis" }),
	foundByQuery: t.Optional(t.Number({ description: "Amount of connections found in the database" })),
	parsingSucceeded: t.Number({ description: "Amount of connections which could be parsed" }),
	parsingFailed: t.Number({ description: "Amount of connections which could not be parsed" }),
	products: t.Record(t.String(), t.Number(), { examples: [{ "Busse": 1, "NahverkehrsonstigeZuege": 2 }], description: "Specifies how often a product has occurred" }),
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

interface NamePart {
	type: string;
	value: string;
}

export type { Station, Stop, NamePart };
