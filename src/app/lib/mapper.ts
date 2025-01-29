import { Connection, Journey, Stop } from "@/app/lib/objects";
import { isMatching } from "./methods";

const normalize = (name?: string): string | undefined => {
    return name?.replace(/[^a-zA-Z0-9]/g, '').toLowerCase();
}

const mapConnections = (
    journeys: Journey[],
    connections: Connection[],
    type: "departures" | "arrivals",
    appendJourneys: boolean = false
): Journey[] => {
    if (appendJourneys) {
        return connections.map(connection => ({ connections: [connection] }));
    }

    return journeys.map((journey: Journey) => {
        const updated = journey?.connections.map((connectionA: Connection) => {
            const matching = connections.find((connectionB: Connection) => isMatching(connectionA, connectionB, type));
            return matching ? { ...connectionA, ...matching } : connectionA;
        });

        return {
            ...journey,
            connections: updated
        }
    });
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
