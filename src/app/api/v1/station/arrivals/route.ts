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
        new Date(a.arrival.actualTime || a.arrival.plannedTime).getTime() -
        new Date(b.arrival.actualTime || b.arrival.plannedTime).getTime()
    );

    return NextResponse.json({entries: sorted}, {status: 200});
}

const vendo = async (id: string, when: string, duration: number, results: number): Promise<Connection[]> => {
    // `dbnav` profile, as `db` does not provide cancelled connections
    const response = await fetch(`https://vendo-prof-dbnav.voldechse.wtf/stops/${id}/arrivals?when=${when}&duration=${duration}&results=${results}`, {method: 'GET'});
    if (!response.ok) return [];

    const data = await response.json();
    if (!data?.arrivals || !Array.isArray(data.arrivals)) return [];

    const map = new Map<string, Connection>();
    data.arrivals.forEach((arrival: any) => {
        const tripId = arrival.tripId;
        if (!tripId || map.has(tripId)) return;

        const connection: Connection = {
            hafas_journeyId: arrival.tripId,
            provenance: arrival.provenance,
            arrival: {
                plannedTime: arrival.plannedWhen,
                actualTime: arrival.when,                     // nullable
                delay: arrival.delay,                         // nullable
                plannedPlatform: arrival.plannedPlatform,     // nullable
                actualPlatform: arrival.platform
            },
            lineInformation: {
                id: arrival.line.id,
                fahrtNr: arrival.line.fahrtNr,
                fullName: arrival.line.name,
                productName: arrival.line.productName
            },
            cancelled: arrival.cancelled ?? false,
        }

        map.set(tripId, connection);
    });

    return Array.from(map.values());
}
