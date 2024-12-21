'use client';

import {useParams} from "next/navigation";
import {useEffect, useState} from "react";
import {Scheduled} from "@/app/lib/schedule";
import Navbar from "@/app/components/navbar";

export default function Departures() {
    const params = useParams();
    const [station, setStation] = useState<{ id: string; name?: string }>({
        id: params.id,
        name: undefined
    });

    const [scheduled, setScheduled] = useState<Scheduled[]>([]);

    useEffect(() => {
        const currentDate = new Date().toISOString();

        // fetch station name from HAFAS
        const fetchStationName = async () => {
            const response = await fetch(`https://v6.db.transport.rest/stations/${station.id}`, {method: 'GET'});
            if (!response.ok) return;

            const data = await response.json();
            setStation((prev) => ({...prev, name: data.name}));
        }

        // fetch departures from HAFAS
        const fetchDepartures = async () => {
            const response = await fetch(`https://hafas.voldechse.wtf/stops/${station.id}/departures?when=${currentDate}&duration=60&results=1000`, {method: 'GET'});
            if (!response.ok) return;

            const data = await response.json();
            if (!data?.departures) return;
            if (!Array.isArray(data.departures)) return;

            const filtered: Scheduled[] = data.departures
                .filter((departure: any) => departure.tripId)
                .map((departure: any) => ({
                    tripId: departure.tripId,
                    plannedWhen: departure.plannedWhen,
                    actualWhen: departure.when,
                    delay: departure.delay,
                    plannedPlatform: departure.plannedPlatform,
                    actualPlatform: departure.platform,
                    directionName: departure.destination.name,
                    directionId: departure.destination.id,
                    line: departure.line
                }));
            setScheduled(filtered);
        }

        fetchStationName();
        fetchDepartures();
    }, []);

    return (
        <div className="text-white">
            <Navbar id={station.id}/>

            <div className="container mx-auto">
                <p>Hey!</p>
            </div>

            {scheduled.length > 0 ? (
                scheduled.map((scheduled: Scheduled) => (
                    <p key={scheduled.tripId}
                       className="py-0.5 px-2 hover:bg-gray-700 hover:cursor-pointer rounded-md"
                    >
                        {scheduled.line.name}
                    </p>
                ))
            ) : (<></>)}
        </div>
    )
}
