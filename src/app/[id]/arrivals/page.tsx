'use client';

import { useParams, useSearchParams } from "next/navigation";
import { useEffect, useRef, useState } from "react";
import ScheduledComponent from "@/app/components/scheduled";
import ScheduledHeader from "@/app/components/scheduled-header";
import { Connection, Journey, Station } from "@/app/lib/objects";
import { mapConnections, sort } from "@/app/lib/mapper";
import { browserLanguage, calculateDuration } from "@/app/lib/methods";
import WingTrain from "@/app/components/wingtrain";
import { DateTime } from "luxon";
import TNavbar from "@/app/components/timetable/TNavbar";
import TClock from "@/app/components/timetable/TClock";

export default function Arrivals() {
    const { id } = useParams();
    const searchParams = useSearchParams();
    const [station, setStation] = useState<Station | undefined>();

    // fetch station
    useEffect(() => {
        const fetchStation = async () => {
            let parsedId = parseInt(id as string);
            if (!parsedId || isNaN(parsedId)) return;

            const response = await fetch(`/api/v1/station`, {
                method: 'POST',
                body: JSON.stringify({ query: parsedId })
            });
            if (!response.ok) return;

            const data = await response.json() as Station;
            setStation(data);
        }

        fetchStation();
    }, []);

    const [startDate, setStartDate] = useState<DateTime>();
    const [endDate, setEndDate] = useState<DateTime>();

    // query "startDate" from URL & update endDate
    useEffect(() => {
        const query = decodeURIComponent(searchParams.get("startDate"));
        let date: DateTime = query ? DateTime.fromISO(query) : DateTime.now();
        if (!date.isValid) date = DateTime.now();

        date = date.set({ second: 0, millisecond: 0 });
        setStartDate(date);
        setEndDate(date.plus({ hours: 1 }));
    }, []);

    const [journeys, setJourneys] = useState<Journey[]>([]);
    const journeysRef = useRef<Journey[]>([]);

    const journeysFromDB = async (): Promise<Journey[]> => {
        const request = await fetch(`/api/v1/bahnhof-proxy?id=${station?.evaNr}&type=arrivals&duration=60&locale=${browserLanguage()}`);
        if (!request.ok) return [];

        const response = await request.json();
        if (!response.entries || !Array.isArray(response.entries)) return [];

        return response.entries as Journey[];
    }

    const updateJourneys = async () => {
        const prevJourneys = journeysRef.current;
        const request = await fetch(`/api/v1/station/arrivals?id=${station?.evaNr}&when=${encodeURIComponent(startDate.toISO())}&duration=${calculateDuration(startDate, endDate, "minutes")}&results=1000`);
        if (!request.ok) return;

        const response = await request.json();
        if (!Array.isArray(response.entries)) return;

        const connections: Connection[] = response.entries as Connection[];

        const mappedJourneys = mapConnections(prevJourneys, connections, "arrivals");
        const sorted = sort(mappedJourneys.journeys, "arrivals");

        setJourneys(sorted);
        journeysRef.current = sorted;
    }

    useEffect(() => {
        if (station === undefined || !station?.evaNr) return;
        setJourneys([]);
        journeysRef.current = [];

        const initJourneys = async () => {
            const fetchedJourneys: Journey[] = await journeysFromDB();
            if (fetchedJourneys.length === 0) return;

            journeysRef.current = fetchedJourneys;
            updateJourneys();
        }
        initJourneys();

        const intervalId = setInterval(updateJourneys, 20 * 1000);
        return () => clearInterval(intervalId);
    }, [station?.evaNr]);

    return (
        <div className="h-screen flex flex-col overflow-hidden md:space-y-4">
            <TNavbar id={station?.evaNr ?? ""} />

            <div className="container mx-auto flex justify-between items-center px-4">
                <span className="text-xl md:text-4xl font-semibold mt-4 px-2 md:px-4">{station?.name ?? ""}</span>
                <TClock className="text-2xl md:text-4xl font-medium mt-4 px-2 md:px-4" />
            </div>

            <ScheduledHeader isDeparture={false} />
            <div className="container mx-auto flex-grow overflow-y-auto scrollbar-hidden">
                {journeys.length > 0 && journeys.map((journey: Journey, index: number) => (
                    <div key={index}>
                        {journey.connections.length === 1 ? (
                            <ScheduledComponent connection={journey.connections[0]} isDeparture={false} renderBorder={true} renderTime={true} />
                        ) : (
                            <WingTrain isDeparture={false} journey={journey} />
                        )}
                    </div>
                ))}
            </div>
        </div>
    )
}
