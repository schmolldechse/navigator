import { Connection, Stop } from "@/app/lib/objects";
import { normalize } from "./mapper";
import { DateTime } from "luxon";

const browserLanguage = (): string => {
    const supported = ['en', 'de'];
    const language = navigator.language.split("-")[0];
    return supported.includes(language) ? language : 'en';
}

const writeName = (stop: Stop, fallbackName: string = ""): string => {
    if (!stop.name) return fallbackName;
    if (!stop.nameParts) return stop.name;
    if (stop.nameParts.length === 0) return stop.name;

    return stop.nameParts.map(part => part.value).join('').trim();
}

const calculateDuration = (
    startDate: DateTime,
    endDate: DateTime,
    unit: "milliseconds" | "seconds" | "minutes" | "hours" | "days"
): number => {
    if (!startDate.isValid || !endDate.isValid) throw new Error("Invalid DateTime objects provided");
    return endDate.diff(startDate, unit).toObject()[unit];
}

const isMatching = (
    connectionA: Connection,
    connectionB: Connection,
    type: "departures" | "arrivals",
    destinationOriginCriteria: boolean = false
): boolean => {
    if (connectionA?.ris_journeyId === connectionB?.ris_journeyId) return true;
    if (connectionA?.hafas_journeyId === connectionB?.hafas_journeyId) return true;

    const aFullName = normalize(connectionA.lineInformation?.fullName);
    const bFullName = normalize(connectionB.lineInformation?.fullName);

    const platformMatch = type === "departures"
        ? connectionA.departure?.plannedPlatform && connectionB.departure?.plannedPlatform
            ? connectionA.departure?.plannedPlatform === connectionB.departure?.plannedPlatform
            : true
        : connectionA.arrival?.plannedPlatform && connectionB.arrival?.plannedPlatform
            ? connectionA.arrival?.plannedPlatform === connectionB.arrival?.plannedPlatform
            : true;

    const timeMatch = type === "departures"
        ? connectionA.departure?.plannedTime === connectionB.departure?.plannedTime
        : connectionA.arrival?.plannedTime === connectionB.arrival?.plannedTime;

    // TODO: documentation why matching with fahrtNr is necessary
    const nameMatch = aFullName === bFullName || aFullName === normalize(connectionB.lineInformation?.fahrtNr);

    const destinationOriginMatch = destinationOriginCriteria
        ? type === "departures"
            ? connectionA?.destination?.id === connectionB?.destination?.id || connectionA?.destination?.name === connectionB?.direction
            : connectionA?.origin?.id === connectionB?.origin?.id || connectionA?.origin?.name === connectionB?.provenance
        : true;

    return (
        platformMatch &&
        timeMatch &&
        nameMatch &&
        destinationOriginMatch
    );
};

/**
 *
 * @param connectionsA - The connections array from `db` profile
 * @param connectionsB - The connections array from `dbnav` profile
 * @param type - Specifies whether to match based on "departures" or "arrivals"
 * @returns A new array of merged connections
 */
const mergeConnections = (
    connectionsA: Connection[],
    connectionsB: Connection[],
    type: "departures" | "arrivals"
): Connection[] => {
    const merge = (connectionA: Connection, connectionB: Connection): Connection => {
        return {
            ris_journeyId: connectionA?.ris_journeyId ?? undefined,
            hafas_journeyId: connectionB?.hafas_journeyId ?? undefined,
            direction: type === "departures" ? (connectionA?.direction || connectionB?.direction) ?? undefined : undefined,
            provenance: type === "arrivals" ? (connectionA?.provenance || connectionB?.provenance) ?? undefined : undefined,
            destination: type === "departures" ? (connectionA?.destination ?? undefined) : undefined, // RIS (`db` profile) does contain this
            origin: type === "arrivals" ? (connectionA?.origin ?? undefined) : undefined, // RIS (`db` profile) does contain this
            departure: type === "departures" ? (connectionA?.departure || connectionB?.departure) : undefined,
            arrival: type === "arrivals" ? (connectionA?.arrival || connectionB?.arrival) : undefined,
            lineInformation: {
                id: (connectionA?.lineInformation?.id || connectionB?.lineInformation?.id) ?? undefined,
                fahrtNr: (connectionA?.lineInformation?.fahrtNr || connectionB?.lineInformation?.fahrtNr) ?? undefined, // HAFAS (`dbnav` profile) contains the line number (e.g. MEX 12)
                fullName: (connectionA?.lineInformation?.fullName || connectionB?.lineInformation?.fullName) ?? undefined,
                product: (connectionA?.lineInformation?.product || connectionB?.lineInformation?.product) ?? undefined,
                operator: connectionA?.lineInformation?.operator ?? undefined
            },
            cancelled: (connectionA?.cancelled || connectionB?.cancelled) ?? false
        }
    }

    const merged: Connection[] = [];
    connectionsB.forEach((connectionB: Connection) => {
        /**
         * additional check for destination/ origin is needed here!
         * for example: a wing-train got 2 connections with the same name ("RE 6") but different locations.
         * it wasn't able to merge one of them, because they had the same name
         */
        const matching = connectionsA.find((connectionA: Connection) => isMatching(connectionA, connectionB, type, true));
        if (!matching) merged.push(connectionB);
        else merged.push(merge(matching, connectionB));
    });
    return merged;
}

export { browserLanguage, writeName, calculateDuration, isMatching, mergeConnections };
