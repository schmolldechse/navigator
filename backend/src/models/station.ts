import type { Time } from "./time.ts";
import type { Message } from "./message.ts";

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

interface StopAnalytics {
	relatedEvaNumbers: number[];
	// total number of connections found in the database
	foundByQuery?: number;
	// total number of connections found with filter being applied
	foundByFilter?: number;
	parsingSucceeded: number;
	parsingFailed: number;
	// how often a product has occurred
	products: Record<string, number>;
	arrival: {
		total: number;

		// in seconds
		averageDelay: number;
		minimumDelay: number;
		maximumDelay: number;

		totalTooEarly: number; // <0 sec
		totalPunctual: number; // 0 - 60 sec
		totalDelayed: number; // >60 sec

		totalPlatformChanges: number;
	};
	departure: {
		total: number;

		// in seconds
		averageDelay: number;
		minimumDelay: number;
		maximumDelay: number;

		totalTooEarly: number; // <0 sec
		totalPunctual: number; // 0 - 60 sec
		totalDelayed: number; // >60 sec

		totalPlatformChanges: number;
	};
	cancellations: number;
}

interface NamePart {
	type: string;
	value: string;
}

export type { Station, Stop, StopAnalytics, NamePart };
