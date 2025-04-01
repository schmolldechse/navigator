import type { Product } from "./products.ts";
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
	locationId?: string;
	coordinates?: {
		latitude: number;
		longitude: number;
	};
	products?: Product[];
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
