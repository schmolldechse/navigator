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
import TFilter from "@/app/components/timetable/TFilter";

export default function Departures() {
    const { id } = useParams();
    const searchParams = useSearchParams();
    const [station, setStation] = useState<Station | undefined>();

    // fetch station
    useEffect(() => {
        const fetchStation = async () => {
            const parsedId = parseInt(id as string);
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

    const [currentFilter, setCurrentFilter] = useState<string[]>();
    const matchesFilter = (journey: Journey) => {
        if (currentFilter === undefined) return true;
        if (currentFilter.includes("*")) return true;

        const firstConnection = journey.connections[0];
        return currentFilter.some(filter =>
            firstConnection.lineInformation?.type === filter ||
            firstConnection.lineInformation?.replacementServiceType === filter ||
            firstConnection.lineInformation?.kind === filter ||
            firstConnection.lineInformation?.product === filter
        );
    }

    const [journeys, setJourneys] = useState<Journey[]>([]);
    const journeysRef = useRef<Journey[]>([]);

    const journeysFromDB = async (): Promise<Journey[]> => {
        const now = DateTime.now().set({ second: 0, millisecond: 0 });
        const durationMinutes = calculateDuration(now, endDate, "minutes");

        // if duration is less or equal than 0, no need to fetch
        if (durationMinutes <= 0) return [];

        // limit duration to a maximum of 6 hours
        const maxDuration = Math.min(durationMinutes, 6 * 60);

        const request = await fetch(`/api/v1/bahnhof-proxy?id=${station?.evaNr}&type=departures&duration=${maxDuration}&locale=${browserLanguage()}`);
        if (!request.ok) return [];

        const response = await request.json();
        if (!response.entries || !Array.isArray(response.entries)) return [];

        return response.entries as Journey[];
    }

    /**
     * Fetches journeys from the `db-vendo-client` and update the journeys
     * @param appendJourneys - if true, append all received journeys. This is helpful, when there's a different date selected than current date
     */
    const updateJourneys = async (appendJourneys: boolean) => {
        const prevJourneys = journeysRef.current;

        // fetch journeys for the full range from startDate to endDate
        const request = await fetch(`/api/v1/station/departures?id=${station?.evaNr}&when=${encodeURIComponent(startDate.toISO())}&duration=${calculateDuration(startDate, endDate, "minutes")}&results=1000`);
        if (!request.ok) return;

        const response = await request.json();
        if (!Array.isArray(response.entries)) return;

        const connections: Connection[] = response.entries as Connection[];

        const mappedJourneys = mapConnections(prevJourneys, connections, "departures", appendJourneys);
        const sorted = sort(mappedJourneys, "departures");

        setJourneys(sorted);
        journeysRef.current = sorted;
    }

    useEffect(() => {
        if (station === undefined || !station?.evaNr) return;
        setJourneys([]);
        journeysRef.current = [];

        const initJourneys = async () => {
            const now = DateTime.now().set({ second: 0, millisecond: 0 });
            const diffInHoursStart = startDate.diff(now, 'hours').hours;
            const diffInHoursEnd = endDate.diff(now, 'hours').hours;

            if (diffInHoursStart < -1 || diffInHoursEnd > 6) {
                await updateJourneys(true);
                return;
            }

            const fetchedJourneys: Journey[] = await journeysFromDB();
            if (fetchedJourneys.length === 0) return;

            journeysRef.current = fetchedJourneys;
            await updateJourneys(false);
        }
        initJourneys();

        const intervalId = setInterval(updateJourneys, 20 * 1000, false);
        return () => clearInterval(intervalId);
    }, [station?.evaNr]);

    return (
        <div className="h-screen flex flex-col overflow-hidden md:space-y-4">
            <TNavbar id={station?.evaNr ?? ""} />

            <div className="container mx-auto flex justify-between items-center px-4">
                <span className="text-xl md:text-4xl font-semibold mt-4 px-2 md:px-4">{station?.name ?? ""}</span>
                <TClock className="text-2xl md:text-4xl font-medium mt-4 px-2 md:px-4" />
            </div>

            <TFilter onSelectType={setCurrentFilter} />

            <ScheduledHeader isDeparture={true} />
            <div className="container mx-auto flex-grow overflow-y-auto scrollbar-hidden">
                {journeys.length > 0 && journeys
                    .filter(matchesFilter)
                    .map((journey: Journey, index: number) => (
                        <div key={index}>
                            {journey.connections.length === 1 ? (
                                <ScheduledComponent
                                    connection={journey.connections[0]}
                                    isDeparture={true}
                                    renderBorder={true}
                                    renderInfo={true}
                                />
                            ) : <WingTrain isDeparture={true} journey={journey} />}
                        </div>
                    ))}
            </div>
        </div>
    )
}
