'use client';

import {useParams} from "next/navigation";
import {useEffect, useState} from "react";
import {Scheduled} from "@/app/lib/schedule";
import Navbar from "@/app/components/navbar";

export default function Departures() {
    const params = useParams();
    const id = params.id;

    const [scheduled, setScheduled] = useState<Scheduled[]>([]);

    useEffect(() => {
        const currentDate = new Date().toISOString();

        // fetch departures from HAFAS
        const fetchDepartures = async () => {
            const response = await fetch(`https://hafas.voldechse.wtf/stops/${id}/departures?when=${currentDate}&duration=60&results=1000`, {method: 'GET'});
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
        
        fetchDepartures();
    }, []);

    return (
        <div className="text-white">
            <Navbar id={id}/>

            <div className="container mx-auto">
                <p>Hey!</p>
            </div>

            <h1 className="text-2xl font-semibold mt-4 px-4">Abfahrten für {id}</h1>

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
