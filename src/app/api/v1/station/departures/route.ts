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
        new Date(a.departure.actualTime || a.departure.plannedTime).getTime() -
        new Date(b.departure.actualTime || b.departure.plannedTime).getTime()
    );

    return NextResponse.json({success: true, entries: sorted}, {status: 200});
}

const v1 = async (id: string, when: string, duration: number, results: number): Promise<Trip[]> => {
    const response = await fetch(`https://hafas-v1.voldechse.wtf/stops/${id}/departures?when=${when}&duration=${duration}&results=${results}`, {method: 'GET'});
    if (!response.ok) return [];

    const data = await response.json();
    if (!data?.departures || !Array.isArray(data.departures)) return [];

    const map = new Map<string, Trip>();
    data.departures.forEach((departure: any) => {
        const tripId = departure.tripId;
        if (!tripId || map.has(tripId)) return;

        const trip: Trip = {
            tripId,
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
                fullName: departure.line.name,
                id: departure.line.id,
                fahrtNr: departure.line.fahrtNr,
                operator: {
                    id: departure.line.operator?.id || '',
                    name: departure.line.operator?.name || ''
                }
            },
            remarks: departure.remarks,
            cancelled: departure.cancelled || false
        }

        map.set(tripId, trip);
    });

    return Array.from(map.values());
}

const v2 = async (trips: Trip[], id: string, when: string, duration: number, results: number): Promise<Trip[]> => {
    const response = await fetch(`https://hafas-v2.voldechse.wtf/stops/${id}/departures?when=${when}duration=${duration}&results=${results}`, {method: 'GET'});
    if (!response.ok) return trips;

    const data = await response.json();
    if (!data?.departures || !Array.isArray(data.departures)) return trips;

    return trips.map((trip: Trip) => {
        const matchingDeparture = data.departures.find((departure: any) => {
            return (
                departure.plannedWhen === trip.departure.plannedTime &&
                departure.line?.fahrtNr === trip.lineInformation?.fahrtNr
            );
        });

        if (matchingDeparture) {
            return {
                ...trip,
                departure: {
                    ...trip.departure,
                    plannedTime: matchingDeparture.plannedWhen,
                    actualTime: matchingDeparture.when,
                    delay: matchingDeparture.delay,
                    plannedPlatform: matchingDeparture.plannedPlatform,
                    actualPlatform: matchingDeparture.platform
                }
            };
        }

        return trip;
    });
}
