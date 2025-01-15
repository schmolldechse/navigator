import {Connection, Journey, Stop} from "@/app/lib/objects";

const normalize = (name?: string): string | undefined => {
    return name?.replace(/[^a-zA-Z0-9]/g, '').toLowerCase();
}

const mapConnections = (journeys: Journey[], connections: Connection[], type: "departures" | "arrivals"): {
    journeys: Journey[],
    faulty: Connection[]
} => {
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

                const platformMatch = type === "departures"
                    ? connection.departure?.plannedPlatform && conn.departure?.plannedPlatform
                        ? connection.departure?.plannedPlatform === conn.departure?.plannedPlatform
                        : true
                    : connection.arrival?.plannedPlatform && conn.arrival?.plannedPlatform
                        ? connection.arrival?.plannedPlatform === conn.arrival?.plannedPlatform
                        : true;

                const timeMatch = type === "departures"
                    ? connection.departure?.plannedTime === conn.departure?.plannedTime
                    : connection.arrival?.plannedTime === conn.arrival?.plannedTime;

                const nameMatch = connectionFullName === connFullName || connectionFullName === normalize(conn.lineInformation?.fahrtNr)

                return (
                    platformMatch &&
                    timeMatch &&
                    nameMatch
                );
            });
            if (!matching) return connection;

            if (matching.ris_journeyId) matched.add(matching.ris_journeyId);
            if (matching.hafas_journeyId) matched.add(matching.hafas_journeyId);

            return {
                ...connection,
                ...matching,
                lineInformation: {
                    ...connection.lineInformation,
                    ...matching.lineInformation
                }
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

const mapStops = (rawData: any): Stop[] => {
    if (!Array.isArray(rawData)) rawData = [rawData];

    return rawData.map((rawStop: any) => {
        const {evaNumber, name, canceled, additional, separation, nameParts} = rawStop;
        return {
            id: evaNumber,
            name: name,
            cancelled: canceled,
            additional: additional || false,
            separation: separation || false,
            nameParts: nameParts ? nameParts.map((part: any) => ({
                type: part.type,
                value: part.value
            })) : [{type: "default", value: name}]
        };
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

export {mapConnections, mapStops, sort};
