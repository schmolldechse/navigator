'use client';

import {useParams} from "next/navigation";
import {useEffect, useState} from "react";
import {Scheduled} from "@/app/lib/schedule";
import Navbar from "@/app/components/navbar";

export default function Departures() {
    const params = useParams();
    const id = params.id;

    const [scheduled, setScheduled] = useState<Scheduled[]>([]);

    // fetch departures from HAFAS
    useEffect(() => {
        const currentUnix = new Date().toISOString();

        const fetchDepartures = async () => {
            const response = await fetch(`https://hafas.voldechse.wtf/stops/${id}/departures?when=${currentUnix}&duration=60&results=1000`, {method: 'GET'});
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
        <>
            <Navbar id={id} />

            {scheduled.length > 0 ? (
                scheduled.map((scheduled: Scheduled) => (
                    <p key={scheduled.tripId}
                       className="text-white py-0.5 px-2 hover:bg-gray-700 hover:cursor-pointer rounded-md"
                    >
                        {scheduled.line.name}
                    </p>
                ))
            ) : ( <></> )}
        </>
    )
}
