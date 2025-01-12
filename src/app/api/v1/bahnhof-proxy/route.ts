import {NextRequest, NextResponse} from "next/server.js";
import {Connection, Journey} from "@/app/lib/objects";
import {mapStops} from "@/app/lib/mapper";

export async function GET(req: NextRequest) {
    const {searchParams} = new URL(req.url);
    const id = searchParams.get("id");

    if (!id) return NextResponse.json({error: 'Id is required'}, {status: 400});

    const duration = searchParams.get("duration") || 60;
    const locale = searchParams.get("locale") || 'en';
    const type = searchParams.get("type");
    if (!type) return NextResponse.json({error: `Type is required. Expected 'departures' | 'arrivals'`})

    const request = await fetch(`https://bahnhof.de/api/boards/${type}?evaNumbers=${id}&duration=${duration}&locale=${locale}`);
    if (!request.ok) return;

    const response = await request.json();
    if (!response.entries || !Array.isArray(response.entries)) return;

    const journeys: Journey[] = [];
    Object.values(response.entries).forEach((journeyRaw: any) => {
        if (!Array.isArray(journeyRaw)) return;

        const journey: Journey = {
            connections: []
        }

        journeyRaw.forEach((connectionRaw: any) => {
            const journeyId = connectionRaw.journeyID;         // RIS journeyId
            if (!journeyId) return;

            const connection: Connection = mapConnection(connectionRaw, type);
            if (!journey.connections.some(existing => existing.ris_journeyId === connection.ris_journeyId)) journey.connections.push(connection);
        });
        journeys.push(journey);
    });

    return NextResponse.json({entries: journeys});
}

const mapConnection = (entry: any, type: string): Connection => {
    const timeSchedule = new Date(entry.timeSchedule).getTime();
    const timeDelayed = new Date(entry.timeDelayed).getTime();

    const delay: number = Math.abs(timeDelayed - timeSchedule) / 1000; // in s

    const isDeparture = type === "departures";

    return {
        ris_journeyId: entry.journeyID,
        destination: isDeparture ? mapStops(entry.destination)[0] : undefined,
        actualDestination: entry.actualDestination ? mapStops(entry.actualDestination)[0] : undefined, // TODO: lookup on arrivals
        origin: !isDeparture ? mapStops(entry.origin)[0] : undefined,
        departure: isDeparture ? {
            plannedTime: entry.timeSchedule,
            actualTime: entry.timeDelayed,
            delay: delay,
            plannedPlatform: entry.platformSchedule,
            actualPlatform: entry.platform
        } : undefined,
        arrival: !isDeparture ? {
            plannedTime: entry.timeSchedule,
            actualTime: entry.timeDelayed,
            delay: delay,
            plannedPlatform: entry.platformSchedule,
            actualPlatform: entry.platform
        } : undefined,
        lineInformation: {
            kind: entry.kind,
            additionalLineName: entry.additionalLineName,
            fullName: entry.lineName,
        },
        viaStops: mapStops(entry.viaStops),
        canceledStopsAfterActualDestination: mapStops(entry.canceledStopsAfterActualDestination), // TODO: lookup on arrivals
        additionalStops: mapStops(entry.additionalStops),
        canceledStops: mapStops(entry.canceledStops),
        messages: entry.messages,
        cancelled: entry.canceled,
    }
}
