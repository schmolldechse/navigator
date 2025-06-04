// TODO:

export interface Sequence {
	track: Track;
	vehicleGroup?: VehicleGroup[];
	direction?: "RIGHT" | "LEFT";
	plannedTrack?: string;
	actualTrack?: string;
}

export interface Track {
	start: { position: number };
	end: { position: number };
	sections: Section[];
	name: string; // name of the track
}

export interface Section {
	name: string;
	start: { position: number };
	end: { position: number };
	gleisabschnittswuerfelPosition: number; // ??????????????????????
	firstClass: boolean;
}

export interface VehicleGroup {
	vehicles: Vehicle[];
	tripReference: TripReference;
	designation: string;
}

export interface TripReference {
	type: string;
	line: string;
	destination: { name: string };
	category: string;
	fahrtNr: number;
}

export interface Vehicle {
	vehicleType: VehicleType;
	status: string;
	orientation: string;
	positionOnTrack: PositionOnTrack;
	equipment: Equipment[];
	orderNumber?: number; // coach number
}

/**
 * category might be:
 *
 * POWERCAR
 * LOCOMOTIVE
 * DOUBLEDECK_*
 * PASSENGERCARRIAGE_*
 * HALDININGCAR
 */
export interface VehicleType {
	category: string;
	model: string;
	firstClass: boolean;
	secondClass: boolean;
}

export interface PositionOnTrack {
	start: { position: number };
	end: { position: number };
	section: string;
}

/**
 * DONE: SEATS_SEVERELY_DISABLED , ZONE_FAMILY , ZONE_QUIET , AIR_CONDITION , TOILET_WHEELCHAIR , WHEELCHAIR_SPACE , BIKE_SPACE
 * TODO: INFO , CABIN_INFANT , SEATS_BAHN_COMFORT
 * UNKNOWN: HALDININGCAR? see VehicleType
 */
export interface Equipment {
	type: string;
	status: string;
}
