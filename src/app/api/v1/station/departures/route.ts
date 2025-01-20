import { NextRequest, NextResponse } from "next/server";
import { Connection } from "@/app/lib/objects";
import { difference } from "@/app/lib/methods";

export async function GET(req: NextRequest) {
    const { searchParams } = new URL(req.url);
    const id = searchParams.get("id");
    if (!id) return NextResponse.json({ error: 'Station id is required' }, { status: 400 });

    const when: Date = searchParams.get("when") ? new Date(searchParams.get("when")) : null;
    if (!when || isNaN(when.getTime())) return NextResponse.json({ error: 'Could not read date' }, { status: 400 });

    const duration = parseInt(searchParams.get("duration")) || 60;
    const results = parseInt(searchParams.get("results")) || 1000;

    const connections = await Promise.all([
        fetchConnections("db", id, when, duration, results),
        fetchConnections("dbnav", id, when, duration, results)
    ]);

    // TODO: match connections together

    /*
    const connections = await vendo(id, when, duration, results);
    if (!connections) return NextResponse.json({entries: []}, {status: 200});

    const sorted = Array.from(connections).sort((a, b) =>
        new Date(a.departure.actualTime || a.departure.plannedTime).getTime() -
        new Date(b.departure.actualTime || b.departure.plannedTime).getTime()
    );
    */

    return NextResponse.json({ entries: connections }, { status: 200 });
}

const fetchConnections = async (
    profile: "db" | "dbnav",
    stationId: string,
    when: Date,
    duration: number,
    results: number
): Promise<Connection[]> => {
    const response = await fetch(`https://vendo-prof-${profile}.voldechse.wtf/stops/${stationId}/departures?when=${when.toISOString()}&duration=${duration}&results=${results}`, { method: 'GET' });
    if (!response.ok) return [];

    const data = await response.json();
    if (!data?.departures || !Array.isArray(data.departures)) return [];

    return mapConnections(profile, data.departures);
}

const mapConnections = async (profile: "db" | "dbnav", data: any): Promise<Connection[]> => {
    const isRIS = profile === "db";

    const map = new Map<string, Connection>();
    data.forEach((departure: any) => {
        const tripId = departure.tripId;
        if (!tripId || map.has(tripId)) return;

        const delay: number = difference(new Date(departure?.plannedwhen), new Date(departure?.when));

        const connection: Connection = {
            hafas_journeyId: !isRIS ? tripId : undefined,
            ris_journeyId: isRIS ? tripId : undefined,
            direction: departure?.direction ?? undefined,
            destination: {
                id: departure?.destination?.id ?? undefined,
                name: departure?.destination?.name ?? undefined,
                cancelled: (departure?.cancelled || departure?.destination?.cancelled) ?? false
            },
            departure: {
                plannedTime: departure?.plannedWhen ?? undefined,
                actualTime: departure?.when ?? undefined,
                delay: departure?.delay ?? delay,
                plannedPlatform: departure?.plannedPlatform ?? undefined,
                actualPlatform: departure?.platform ?? undefined
            },
            lineInformation: {
                id: departure.line?.id ?? undefined,
                fahrtNr: departure.line?.fahrtNr ?? undefined,
                fullName: departure.line?.name ?? undefined,
                product: departure.line?.product ?? undefined,
                operator: {
                    id: departure.line?.operator?.id ?? undefined,
                    name: departure.line?.operator?.name ?? undefined
                }
            },
            cancelled: departure.cancelled ?? false
        }

        map.set(tripId, connection);
    });
    return Array.from(map.values());
}
