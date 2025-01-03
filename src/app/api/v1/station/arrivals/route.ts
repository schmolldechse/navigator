import {NextRequest, NextResponse} from "next/server";
import {Trip} from "@/app/lib/trip";

export async function GET(req: NextRequest) {
    const {searchParams} = new URL(req.url);
    const id = searchParams.get("id");

    if (!id) return NextResponse.json({success: false, error: 'Station id is required'}, {status: 400});

    const when = searchParams.get("when") || new Date().toISOString();
    const duration = parseInt(searchParams.get("duration")) || 60;
    const results = parseInt(searchParams.get("results")) || 1000;

    const trips = await v1(id, when, duration, results);
    if (!trips) return NextResponse.json({success: true, entries: []}, {status: 200});

    const updatedTrips = await v2(trips, id, when, duration, results);

    const sorted = Array.from(updatedTrips.values()).sort((a, b) =>
        new Date(a.arrival.actualTime || a.arrival.plannedTime).getTime() -
        new Date(b.arrival.actualTime || b.arrival.plannedTime).getTime()
    );

    return NextResponse.json({success: true, entries: sorted}, {status: 200});
}

const v1 = async (id: string, when: string, duration: number, results: number): Promise<Trip[]> => {
    const response = await fetch(`https://hafas-v1.voldechse.wtf/stops/${id}/arrivals?when=${when}&duration=${duration}&results=${results}`, {method: 'GET'});
    if (!response.ok) return [];

    const data = await response.json();
    if (!data?.arrivals || !Array.isArray(data.arrivals)) return [];

    const map = new Map<string, Trip>();
    data.arrivals.forEach((arrival: any) => {
        const tripId = arrival.tripId;
        if (!tripId || map.has(tripId)) return;

        const trip: Trip = {
            tripId,
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
            },
            remarks: arrival.remarks,
            cancelled: arrival.cancelled || false
        }

        map.set(tripId, trip);
    });

    return Array.from(map.values());
}

const v2 = async (trips: Trip[], id: string, when: string, duration: number, results: number): Promise<Trip[]> => {
    const response = await fetch(`https://hafas-v2.voldechse.wtf/stops/${id}/arrivals?when=${when}duration=${duration}&results=${results}`, {method: 'GET'});
    if (!response.ok) return trips;

    const data = await response.json();
    if (!data?.arrivals || !Array.isArray(data.arrivals)) return trips;

    return trips.map((trip: Trip) => {
        const matchingArrival = data.arrivals.find((arrival: any) => {
            return (
                arrival.plannedWhen === trip.arrival.plannedTime &&
                arrival.line?.fahrtNr === trip.lineInformation?.fahrtNr
            );
        });

        if (matchingArrival) {
            return {
                ...trip,
                arrival: {
                    ...trip.arrival,
                    plannedTime: matchingArrival.plannedWhen,
                    actualTime: matchingArrival.when,
                    delay: matchingArrival.delay,
                    plannedPlatform: matchingArrival.plannedPlatform,
                    actualPlatform: matchingArrival.platform
                }
            };
        }

        return trip;
    });
}
