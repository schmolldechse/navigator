import {NextRequest, NextResponse} from "next/server";
import {Connection} from "@/app/lib/objects";

export async function GET(req: NextRequest) {
    const {searchParams} = new URL(req.url);
    const id = searchParams.get("id");

    if (!id) return NextResponse.json({success: false, error: 'Station id is required'}, {status: 400});

    const when = searchParams.get("when") || new Date().toISOString();
    const duration = parseInt(searchParams.get("duration")) || 60;
    const results = parseInt(searchParams.get("results")) || 1000;

    const connectionsDB = await vendoDB(id, when, duration, results);
    if (!connectionsDB) return NextResponse.json({entries: []}, {status: 200});

    const connectionsDBNav = await vendoDBNav(id, when, duration, results);
    const sorted = Array.from(mapConnections(connectionsDB, connectionsDBNav)).sort((a, b) =>
        new Date(a.departure.actualTime || a.departure.plannedTime).getTime() -
        new Date(b.departure.actualTime || b.departure.plannedTime).getTime()
    );

    return NextResponse.json({entries: sorted}, {status: 200});
}

const vendoDB = async (id: string, when: string, duration: number, results: number): Promise<Connection[]> => {
    const response = await fetch(`https://vendo-prof-db.voldechse.wtf/stops/${id}/departures?when=${when}&duration=${duration}&results=${results}`, {method: 'GET'});
    if (!response.ok) return [];

    const data = await response.json();
    if (!data?.departures || !Array.isArray(data.departures)) return [];

    const map = new Map<string, Connection>();
    data.departures.forEach((departure: any) => {
        const tripId = departure.tripId;
        if (!tripId || map.has(tripId)) return;

        const connection: Connection = {
            ris_journeyId: departure.tripId,
            destination: {
                id: departure.destination.id,
                name: departure.destination.name
            },
            departure: {
                plannedTime: departure.plannedWhen,
                actualTime: departure.when,                     // nullable
                delay: departure.delay,                         // nullable
                plannedPlatform: departure.plannedPlatform,     // nullable
                actualPlatform: departure.platform
            },
            lineInformation: {
                productName: departure.line.productName,
                id: departure.line.id,
                fahrtNr: departure.line.fahrtNr,
                name: departure.line.name,
                operator: {
                    id: departure.line.operator?.id || '',
                    name: departure.line.operator?.name || ''
                }
            }
        }

        map.set(tripId, connection);
    });

    return Array.from(map.values());
}

const vendoDBNav = async (id: string, when: string, duration: number, results: number): Promise<Connection[]> => {
    const response = await fetch(`https://vendo-prof-dbnav.voldechse.wtf/stops/${id}/departures?when=${when}&duration=${duration}&results=${results}`, {method: 'GET'});
    if (!response.ok) return [];

    const data = await response.json();
    if (!data?.departures || !Array.isArray(data.departures)) return [];

    const map = new Map<string, Connection>();
    data.departures.forEach((departure: any) => {
        const tripId = departure.tripId;
        if (!tripId || map.has(tripId)) return;

        const connection: Connection = {
            hafas_journeyId: departure.tripId,
            departure: {
                plannedTime: departure.plannedWhen,
                actualTime: departure.when,                     // nullable
                delay: departure.delay,                         // nullable
                plannedPlatform: departure.plannedPlatform,     // nullable
                actualPlatform: departure.platform
            },
            lineInformation: {
                id: departure.line.id,
                name: departure.line.name
            }
        }

        map.set(tripId, connection);
    });

    return Array.from(map.values());
}

const mapConnections = (mapV1: Connection[], mapV2: Connection[]): Connection[] => {
    return mapV1.map((connectionV1: Connection) => {
        const matching = mapV2.find((connectionV2: Connection) =>
            connectionV1.departure.plannedTime === connectionV2.departure.plannedTime &&
            connectionV1.departure.plannedPlatform === connectionV2.departure.plannedPlatform &&
            connectionV1.lineInformation?.name === connectionV2.lineInformation?.name &&
            connectionV1.lineInformation?.id.includes(connectionV2.lineInformation?.id)
        );

        if (matching) {
            return {
                ...connectionV1,
                hafas_journeyId: matching.hafas_journeyId,
                departure: {
                    ...connectionV1.departure,
                    plannedTime: matching.departure.plannedTime,
                    actualTime: matching.departure.actualTime,
                    delay: matching.departure.delay,
                    plannedPlatform: matching.departure.plannedPlatform,
                    actualPlatform: matching.departure.actualPlatform
                }
            };
        }

        return connectionV1;
    });

}
