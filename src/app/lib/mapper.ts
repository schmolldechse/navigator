import { Connection, Journey, Stop } from "@/app/lib/objects";
import { isMatching } from "./methods";
import { DateTime } from "luxon";

const normalize = (name?: string): string | undefined => {
    return name?.replace(/[^a-zA-Z0-9]/g, '').toLowerCase();
}

const mapConnections = (
    journeys: Journey[],
    connections: Connection[],
    type: "departures" | "arrivals"
): Journey[] => {
    const matched: { hafas_journeyId: string }[] = [];

    const updatedJourneys = journeys.map((journey: Journey) => ({
        ...journey,
        connections: journey.connections.map((connectionA: Connection) => {
            // connectionA      - from RIS
            // connectionB      - from HAFAS
            const matching = connections.find((connectionB: Connection) => {
                const matchesRIS = connectionA.ris_journeyId && (connectionA.ris_journeyId === connectionB.ris_journeyId);
                const matchesHAFAS = connectionA.hafas_journeyId && (connectionA.hafas_journeyId === connectionB.hafas_journeyId);

                if (matchesRIS || matchesHAFAS) return true;
                if (isMatching(connectionA, connectionB, type)) return true;
                return false;
            });
            if (!matching) return connectionA;

            matched.push({ hafas_journeyId: matching?.hafas_journeyId });
            return {
                ...connectionA,
                ...matching,
                lineInformation: {
                    ...connectionA.lineInformation,
                    ...matching.lineInformation
                }
            };
        })
    }));

    // sort out the connections that do not have an assigned journey
    // this may happen if, for example, a train is running with an "lineInformation.additionalLineName" and no second id can be found
    const faulty = connections.filter(connection => !matched.some(m => m.hafas_journeyId === connection?.hafas_journeyId));
    const newJourneys = faulty.map(connection => ({ connections: [connection] }));

    return [...updatedJourneys, ...newJourneys]
}

const sort = (journeys: Journey[], type: "departures" | "arrivals"): Journey[] => {
    return journeys.sort((a, b) => {
        const earliest = (journey: Journey): string => {
            const times = journey.connections
                .map(conn => type === "departures"
                    ? conn.departure?.actualTime || conn.departure?.plannedTime
                    : conn.arrival?.actualTime || conn.arrival?.plannedTime)
            return times.length > 0 ? times.sort()[0] : "9999-12-31T23:59:59";
        };

        const timeA = earliest(a);
        const timeB = earliest(b);

        return timeA.localeCompare(timeB);
    })
}

export {normalize, mapConnections, sort};
