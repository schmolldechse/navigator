export type Trip = {
    tripId: string,
    origin?: {
        id: string,
        name: string
    },
    destination?: {
        id: string,
        name: string
    },
    ueber?: string[],               // the stops where the train stops
    departure?: {
        plannedTime: string,        // the time when the train should have departed
        actualTime: string,         // the time when the train actually departed
        delay: string,              // the delay of departure in sec
        plannedPlatform: string,
        actualPlatform: string
    }
    arrival?: {
        plannedTime: string,        // the time when the train should have been at the end station
        actualTime: string,         // the time when the train reaches his end station in reality
        delay: string,              // the delay of arrival in sec
        plannedPlatform: string,
        actualPlatform: string
    },
    lineInformation?: {
        productName: string,        // the type of train service operarting (e.g., RB, RE, IRE)
        fullName: string,           // the name of the train line in operation (e.g. RE 7)
        id: string,                 // the line number, often the same as fullName but with hypens ("-") instead of spaces
        fahrtNr: string,            // the train journey number, a unique identifier for a specific trip
        operator?: {
            id: string,             // the id of the railway company to identify the train's color
            name: string            // the name of the railway company (EVU) operating the train
        }
    },
    currentLocation?: {
        latitude: number,
        longitude: number
    },
    stopovers?: [],
    remarks?: [],
    cancelled?: boolean,
    loadFactor?: string
}
