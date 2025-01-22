import { NextRequest, NextResponse } from "next/server";
import { Connection } from "@/app/lib/objects";
import { DateTime } from "luxon";
import { calculateDuration, mergeConnections } from "@/app/lib/methods";

export async function GET(req: NextRequest) {
    const { searchParams } = new URL(req.url);
    const id = searchParams.get("id");
    if (!id) return NextResponse.json({ error: 'Station id is required' }, { status: 400 });

    const whenParam: string = decodeURIComponent(searchParams.get("when"));
    let when: DateTime = whenParam ? DateTime.fromISO(whenParam) : DateTime.now();
    if (!when.isValid) return NextResponse.json({ error: 'Could not read date' }, { status: 400 });
    when = when.set({ second: 0, millisecond: 0 });

    const duration = parseInt(searchParams.get("duration")) || 60;
    const results = parseInt(searchParams.get("results")) || 1000;

    const [db, dbnav] = await Promise.all([
        fetchConnections("db", id, when, duration, results),
        fetchConnections("dbnav", id, when, duration, results)
    ]);

    const connections = mergeConnections(db, dbnav, "arrivals");
    if (!connections) return NextResponse.json({ entries: [] }, { status: 200});

    const sorted = Array.from(connections).sort((a, b) =>
        new Date(a.arrival.actualTime || a.arrival.plannedTime).getTime() -
        new Date(b.arrival.actualTime || b.arrival.plannedTime).getTime()
    );

    return NextResponse.json({ entries: sorted }, { status: 200 });
}

const fetchConnections = async (
    profile: "db" | "dbnav",
    stationId: string,
    when: DateTime,
    duration: number,
    results: number
): Promise<Connection[]> => {
    const response = await fetch(`https://vendo-prof-${profile}.voldechse.wtf/stops/${stationId}/arrivals?when=${encodeURIComponent(when.toISO())}&duration=${duration}&results=${results}`, { method: 'GET' });
    if (!response.ok) return [];

    const data = await response.json();
    if (!data?.arrivals || !Array.isArray(data.arrivals)) return [];

    return mapConnections(profile, data.arrivals);
}

const mapConnections = async (profile: "db" | "dbnav", data: any): Promise<Connection[]> => {
    const isRIS = profile === "db";

    const map = new Map<string, Connection>();
    data.forEach((arrival: any) => {
        const tripId = arrival.tripId;
        if (!tripId || map.has(tripId)) return;

        const delay: number = calculateDuration(DateTime.fromISO(arrival?.when || arrival?.plannedWhen), DateTime.fromISO(arrival?.plannedWhen), "seconds");

        const connection: Connection = {
            hafas_journeyId: !isRIS ? tripId : undefined,
            ris_journeyId: isRIS ? tripId : undefined,
            provenance: arrival?.provenance ?? undefined,
            origin: {
                id: arrival?.origin?.id ?? undefined,
                name: arrival?.origin?.name ?? undefined,
                cancelled: (arrival?.cancelled || arrival?.origin?.cancelled) ?? false
            },
            arrival: {
                plannedTime: arrival?.plannedWhen ?? undefined,
                actualTime: (arrival?.when || arrival?.plannedWhen) ?? undefined,
                delay: arrival?.delay ?? delay,
                plannedPlatform: arrival?.plannedPlatform ?? undefined,
                actualPlatform: arrival?.platform ?? undefined
            },
            lineInformation: {
                id: arrival.line?.id ?? undefined,
                fahrtNr: arrival.line?.fahrtNr ?? undefined,
                fullName: arrival.line?.name ?? undefined,
                product: arrival.line?.product ?? undefined,
                operator: {
                    id: arrival.line?.operator?.id ?? undefined,
                    name: arrival.line?.operator?.name ?? undefined
                }
            },
            cancelled: arrival.cancelled ?? false
        }

        map.set(tripId, connection);
    });
    return Array.from(map.values());
}