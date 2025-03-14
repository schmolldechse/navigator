import type { Stop } from "./station.ts";
import type { Time } from "./time.ts";
import type { Message } from "./message.ts";

export interface Route {
	earlierRef: string;
	laterRef: string;
	journeys: Journey[];
}

export interface Journey {
	connections: Connection[];
	refreshToken?: string;
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
		type?: string; // see product.ts for value
		product?: string; // product category like 'MEX', 'RB', 'RE', ...
		replacementServiceType?: string;
		lineName?: string;
		additionalLineName?: string;
		fahrtNr?: string;
		operator?: {
			id?: string;
			name?: string;
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
