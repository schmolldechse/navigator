'use client';

import {useParams} from "next/navigation";
import {useEffect, useState} from "react";
import {Scheduled} from "@/app/lib/schedule";
import Navbar from "@/app/components/navbar";
import Clock from "@/app/components/clock";

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

            const scheduledItems: Scheduled[] = data.departures
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

            const filtered = Array.from(
                new Map(scheduledItems.map((scheduled: Scheduled) => [scheduled.tripId, scheduled])).values()
            );
            setScheduled(filtered);
        }

        fetchStationName();
        fetchDepartures();
    }, []);

    return (
        <div className="text-white">
            <Navbar id={station.id}/>

            <div className="container mx-auto flex justify-between items-center mt-4 px-4">
                <span className="text-4xl font-semibold mt-4 px-4">{station.name}</span>
                <Clock className="text-4xl font-semibold mt-4 px-4"/>
            </div>

            {scheduled.length > 0 ? (
                scheduled.map((scheduled: Scheduled) => (
                    <p key={scheduled.tripId}
                       className="py-0.5 px-2 hover:bg-gray-700 hover:cursor-pointer rounded-md"
                    >
                        {scheduled.line?.name} : {scheduled.tripId}
                    </p>
                ))
            ) : (<></>)}
        </div>
    )
}
