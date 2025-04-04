import type { Stop } from "./station.ts";
import type { Time } from "./time.ts";
import type { Message } from "./message.ts";

interface Journey {
	connections: Connection[];
}

interface Connection {
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
	// cancelledStopsAfterActualDestination?: Stop[];
	// additionalStops?: Stop[];
	// cancelledStops?: Stop[];
	messages?: Message[];
	cancelled?: boolean;
	providesVehicleSequence?: boolean;
	walking?: boolean;

	// in meter
	distance?: number;

	/**
	 * low-to-medium
	 * high
	 * very-high
	 * exceptionally-high
	 */
	loadFactor?: string;
}

interface LineColor {
	lineName: string;
	hafasLineId: string;
	hafasOperatorCode: string;
	backgroundColor: string;
	textColor: string;
	borderColor: string;
	shape: string;
}

export type { Journey, Connection, LineColor };
