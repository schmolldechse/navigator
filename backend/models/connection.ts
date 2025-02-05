export interface Journey {
    connections: Connection[];
}

export interface Connection {
    ris_journeyId?: string;
    hafas_journeyId?: string;
    origin?: Stop;
    provenance?: string;
    destination?: Stop;
    direction?: string;
    viaStops?: Stop[];
    departure?: Time;
    arrival?: Time;
    lineInformation?: {
        type?: string;
        replacementServiceType?: string;
        lineName?: string;
        additionalLineName?: string;
        fahrtNr?: string;
        operator?: {
            id: string;
            name: string
        }
    };
    cancelledStopsAfterActualDestination?: Stop[];
    additionalStops?: Stop[];
    cancelledStops?: Stop[];
    messages?: Messages;
    cancelled?: boolean;
    providesVehicleSequence?: boolean
}