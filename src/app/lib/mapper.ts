import {Connection, Journey} from "@/app/lib/objects";

const normalize = (name?: string): string | undefined => {
    return name?.replace(/\s+/g, '').toLowerCase();
}

const mapConnections = (journeys: Journey[], connections: Connection[]): { journeys: Journey[], faulty: Connection[] } => {
    const matched = new Set<string>();

    const updatedJourneys = journeys.map((journey: Journey) => ({
        ...journey,
        connections: journey.connections.map((connection: Connection) => {
            // connection   - from RIS
            // conn         - from HAFAS
            const matching = connections.find((conn: Connection) => {
                if (connection.hafas_journeyId && (connection.hafas_journeyId === conn.hafas_journeyId)) return true;

                const connectionFullName = normalize(connection.lineInformation?.fullName);
                const connFullName = normalize(conn.lineInformation?.fullName);

                const platformMatch = connection.departure?.plannedPlatform && conn.departure?.plannedPlatform
                    ? connection.departure?.plannedPlatform === conn.departure?.plannedPlatform
                    : true;

                return (
                    platformMatch &&
                    connection.departure?.plannedTime === conn.departure?.plannedTime &&
                    (connectionFullName === connFullName || connFullName?.includes(connectionFullName ?? '') || connectionFullName?.includes(connFullName ?? '')) &&
                    connection.destination?.name === conn.direction
                );
            });
            if (!matching) return connection;

            if (matching.ris_journeyId) matched.add(matching.ris_journeyId);
            if (matching.hafas_journeyId) matched.add(matching.hafas_journeyId);

            return {
                ...connection,
                ...matching,
            };
        })
    }));

    // sort out the connections that do not have an assigned journey
    // this may happen if, for example, a train is running with an "lineInformation.additionalLineName" and no second id can be found
    const faulty = connections.filter(conn => !matched.has(conn.ris_journeyId ?? '') && !matched.has(conn.hafas_journeyId ?? ''));
    return {journeys: updatedJourneys, faulty: faulty};

    /**
     // handle Connection's which have no Journey
     const newJourneys = connections.filter(conn => !matched.has(conn.ris_journeyId ?? '') && !matched.has(conn.hafas_journeyId ?? ''))
     .map(conn => ({connections: [conn]}));

     return [...updatedJourneys, ...newJourneys];
     */
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
