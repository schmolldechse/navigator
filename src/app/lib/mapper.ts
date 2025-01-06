import {Connection, Journey} from "@/app/lib/objects";

const mapConnections = (journeys: Journey[], connections: Connection[]): Journey[] => {
    const matched = new Set<string>();

    const updatedJourneys = journeys.map((journey: Journey) => ({
        ...journey,
        connections: journey.connections.map(connection => {
            const matching = connections.find(conn => conn.ris_journeyId === connection.ris_journeyId || conn.hafas_journeyId === connection.hafas_journeyId);
            if (!matching) return connection;

            if (matching.ris_journeyId) matched.add(matching.ris_journeyId);
            if (matching.hafas_journeyId) matched.add(matching.hafas_journeyId);

            return {
                ...connection,
                ...matching,
                lineInformation: {
                    ...connection.lineInformation,
                    ...matching.lineInformation,
                    kind: connection.lineInformation?.kind ?? matching.lineInformation?.kind
                }
            };
        })
    }));

    // handle Connection's which have no Journey
    const newJourneys = connections.filter(conn => !matched.has(conn.ris_journeyId ?? '') && !matched.has(conn.hafas_journeyId ?? ''))
        .map(conn => ({connections: [conn]}));

    return [...updatedJourneys, ...newJourneys];
}

const sort = (journeys: Journey[]): Journey[] => {
    return journeys.sort((a, b) => {
        const earliest = (journey: Journey): string => {
            const times = journey.connections
                .map(conn => conn.departure?.actualTime || conn.departure?.plannedTime)
                .filter((time): time is string => !!time);
            return times.length > 0 ? times.sort()[0] : "9999-12-31T23:59:59";
        };

        const timeA = earliest(a);
        const timeB = earliest(b);

        return timeA.localeCompare(timeB);
    })
}

export {mapConnections, sort};
