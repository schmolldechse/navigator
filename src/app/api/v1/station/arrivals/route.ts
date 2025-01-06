import {NextRequest, NextResponse} from "next/server";
import {Connection} from "@/app/lib/objects";

export async function GET(req: NextRequest) {
    const {searchParams} = new URL(req.url);
    const id = searchParams.get("id");

    if (!id) return NextResponse.json({success: false, error: 'Station id is required'}, {status: 400});

    const when = searchParams.get("when") || new Date().toISOString();
    const duration = parseInt(searchParams.get("duration")) || 60;
    const results = parseInt(searchParams.get("results")) || 1000;

    const connectionsV1 = await v1(id, when, duration, results);
    if (!connectionsV1) return NextResponse.json({entries: []}, {status: 200});

    const connectionsV2 = await v2(id, when, duration, results);

    const sorted = Array.from(mapConnections(connectionsV1, connectionsV2)).sort((a, b) =>
        new Date(a.arrival.actualTime || a.arrival.plannedTime).getTime() -
        new Date(b.arrival.actualTime || b.arrival.plannedTime).getTime()
    );

    return NextResponse.json({entries: sorted}, {status: 200});
}

const v1 = async (id: string, when: string, duration: number, results: number): Promise<Connection[]> => {
    const response = await fetch(`https://hafas-v1.voldechse.wtf/stops/${id}/arrivals?when=${when}&duration=${duration}&results=${results}`, {method: 'GET'});
    if (!response.ok) return [];

    const data = await response.json();
    if (!data?.arrivals || !Array.isArray(data.arrivals)) return [];

    const map = new Map<string, Connection>();
    data.arrivals.forEach((arrival: any) => {
        const tripId = arrival.tripId;
        if (!tripId || map.has(tripId)) return;

        const connection: Connection = {
            tripId: arrival.tripId,
            hafas_journeyId: arrival.tripId,
            origin: {
                id: arrival.origin.id,
                name: arrival.origin.name
            },
            arrival: {
                plannedTime: arrival.plannedWhen,
                actualTime: arrival.when,                     // nullable
                delay: arrival.delay,                         // nullable
                plannedPlatform: arrival.plannedPlatform,     // nullable
                actualPlatform: arrival.platform
            },
            lineInformation: {
                productName: arrival.line.productName,
                fullName: arrival.line.name,
                id: arrival.line.id,
                fahrtNr: arrival.line.fahrtNr,
                operator: {
                    id: arrival.line.operator?.id || '',
                    name: arrival.line.operator?.name || ''
                }
            }
        }

        map.set(tripId, connection);
    });

    return Array.from(map.values());
}

const v2 = async (id: string, when: string, duration: number, results: number): Promise<Connection[]> => {
    const response = await fetch(`https://hafas-v2.voldechse.wtf/stops/${id}/arrivals?when=${when}&duration=${duration}&results=${results}`, {method: 'GET'});
    if (!response.ok) return [];

    const data = await response.json();
    if (!data?.arrivals || !Array.isArray(data.arrivals)) return [];

    const map = new Map<string, Connection>();
    data.arrivals.forEach((arrival: any) => {
        const tripId = arrival.tripId;
        if (!tripId || map.has(tripId)) return;

        const connection: Connection = {
            ris_journeyId: arrival.tripId,
            arrival: {
                plannedTime: arrival.plannedWhen,
                actualTime: arrival.when,                     // nullable
                delay: arrival.delay,                         // nullable
                plannedPlatform: arrival.plannedPlatform,     // nullable
                actualPlatform: arrival.platform
            },
            lineInformation: {
                fahrtNr: arrival.line.fahrtNr,
            }
        }

        map.set(tripId, connection);
    });

    return Array.from(map.values());
}

const mapConnections = (mapV1: Connection[], mapV2: Connection[]): Connection[] => {
    return mapV1.map((connectionV1: Connection) => {
        const matching = mapV2.find((connectionV2) =>
            connectionV1.arrival.plannedTime === connectionV2.arrival.plannedTime &&
            connectionV1.lineInformation?.fahrtNr === connectionV2.lineInformation?.fahrtNr
        );

        if (matching) {
            return {
                ...connectionV1,
                ris_journeyId: matching.ris_journeyId,
                arrival: {
                    ...connectionV1.arrival,
                    plannedTime: matching.arrival.plannedTime,
                    actualTime: matching.arrival.actualTime,
                    delay: matching.arrival.delay,
                    plannedPlatform: matching.arrival.plannedPlatform,
                    actualPlatform: matching.arrival.actualPlatform
                }
            };
        }

        return connectionV1;
    });

}
