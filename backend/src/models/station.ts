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

interface NamePart {
	type: string;
	value: string;
}

export type { Station, Stop, NamePart };
