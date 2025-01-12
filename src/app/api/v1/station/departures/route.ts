import {NextRequest, NextResponse} from "next/server";
import {Connection} from "@/app/lib/objects";

export async function GET(req: NextRequest) {
    const {searchParams} = new URL(req.url);
    const id = searchParams.get("id");

    if (!id) return NextResponse.json({success: false, error: 'Station id is required'}, {status: 400});

    const when = searchParams.get("when") || new Date().toISOString();
    const duration = parseInt(searchParams.get("duration")) || 60;
    const results = parseInt(searchParams.get("results")) || 1000;

    const connections = await vendo(id, when, duration, results);
    if (!connections) return NextResponse.json({entries: []}, {status: 200});

    const sorted = Array.from(connections).sort((a, b) =>
        new Date(a.departure.actualTime || a.departure.plannedTime).getTime() -
        new Date(b.departure.actualTime || b.departure.plannedTime).getTime()
    );

    return NextResponse.json({entries: sorted}, {status: 200});
}

const vendo = async (id: string, when: string, duration: number, results: number): Promise<Connection[]> => {
    // `dbnav` profile, as `db` does not provide cancelled connections
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
            direction: departure.direction,
            departure: {
                plannedTime: departure.plannedWhen,
                actualTime: departure.when,                     // nullable
                delay: departure.delay,                         // nullable
                plannedPlatform: departure.plannedPlatform,     // nullable
                actualPlatform: departure.platform
            },
            lineInformation: {
                id: departure.line.id,
                fahrtNr: departure.line.fahrtNr,
                fullName: departure.line.name,
                productName: departure.line.productName,
            }
        }

        map.set(tripId, connection);
    });

    return Array.from(map.values());
}
