export type Connection = {
    ris_journeyId?: string,          // RIS identifier   (DB RIS internal, HAFAS v2)
    hafas_journeyId?: string,        // HAFAS identifier (HAFAS v1)
    origin?: Stop,
    destination?: Stop,
    actualDestination?: Stop,
    walking?: boolean,
    direction?: string,                 // the stop where the line is going
    provenance?: string,                 // the stop where the line comes from
    viaStops?: Stop[],                  // the stops where the line stops
    departure?: {
        plannedTime: string,        // the time when the train should have departed
        actualTime: string,         // the time when the train actually departed
        delay: number,              // the delay of departure in sec
        plannedPlatform: string,
        actualPlatform: string
    }
    arrival?: {
        plannedTime: string,        // the time when the train should have been at the end station
        actualTime: string,         // the time when the train reaches his end station in reality
        delay: number,              // the delay of arrival in sec
        plannedPlatform: string,
        actualPlatform: string
    },
    lineInformation?: {
        kind?: string,              // the type of train service operating (from DB RIS)
        additionalLineName?: string, // an additional name of the train line in operation (e.g. IC 387 is running simultaneously as RE 87)
        productName?: string,       // the type of train service operarting (e.g., RB, RE, IRE)
        fullName?: string,          // the name of the train line in operation (e.g. RE 7)
        id?: string,                // the line number, often the same as fullName but with hypens ("-") instead of spaces
        fahrtNr?: string,           // the train journey number, a unique identifier for a specific trip
        operator?: {
            id: string,             // the id of the railway company to identify the train's color
            name: string            // the name of the railway company (EVU) operating the train
        },
        product?: string,           // the type of line (e.g. bus, suburban, national, regional)
    },
    currentLocation?: {
        latitude: number,
        longitude: number
    },
    canceledStopsAfterActualDestination?: Stop[],
    additionalStops?: Stop[],
    canceledStops?: Stop[],
    stopovers?: [],
    remarks?: [],
    messages?: any,
    cancelled?: boolean,
    loadFactor?: string
}

export type Stop = {
    id: string,
    name: string,
    cancelled: boolean,
    additional?: boolean,
    separation?: boolean,
    nameParts?: NamePart[]
}

export type NamePart = {
    type: string,
    value: string
}

// a journey can hold multiple connections, e.g. if the train parts are separated later
export type Journey = {
    connections: Connection[]
}

export type Station = {
    name: string,
    locationId: string,
    evaNr: string,
    coordinates: {
        latitude: number,
        longitude: number
    },
    products: any[]
}
