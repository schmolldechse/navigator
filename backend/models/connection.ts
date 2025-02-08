import { Stop } from "./station.ts";
import { Time } from "./time.ts";
import { Message } from "./message.ts";

export interface Journey {
	connections: Connection[];
}

export interface Connection {
	ris_journeyId?: string;
	hafas_journeyId?: string;
	origin?: Stop;
	provenance?: string;
	destination?: Stop;
	actualDestination?: Stop;
	direction?: string;
	viaStops?: Stop[];
	departure?: Time;
	arrival?: Time;
	lineInformation?: {
		type?: string;					// from Bahnhof API, uppercase
		product?: string;				// from Vendo, lowercase
		replacementServiceType?: string;
		lineName?: string;
		additionalLineName?: string;
		fahrtNr?: string;
		operator?: {
			id: string;
			name: string;
		};
	};
	cancelledStopsAfterActualDestination?: Stop[];
	additionalStops?: Stop[];
	cancelledStops?: Stop[];
	messages?: {
		common: Message[];
		delay: Message[];
		cancellation: Message[];
		destination: Message[];
		via: Message[];
	};
	cancelled?: boolean;
	providesVehicleSequence?: boolean;
}
