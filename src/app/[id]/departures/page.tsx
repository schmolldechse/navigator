'use client';

import {useParams} from "next/navigation";
import {useEffect, useRef, useState} from "react";
import Navbar from "@/app/components/navbar";
import Clock from "@/app/components/clock";
import ScheduledComponent from "@/app/components/scheduled";
import ScheduledHeader from "@/app/components/scheduled-header";
import {Connection, Journey, Station} from "@/app/lib/objects";
import {mapConnections, sort} from "@/app/lib/mapper";

export default function Departures() {
    const params = useParams();
    const [station, setStation] = useState<Station | undefined>();

    // fetch station
    useEffect(() => {
        const fetchStation = async () => {
            let paramsId = Array.isArray(params.id) ? params.id[0] : params.id || "";
            let id = parseInt(paramsId);
            if (isNaN(id)) return undefined;

            const response = await fetch(`/api/v1/station`, {
                method: 'POST',
                body: JSON.stringify({query: id})
            });
            if (!response.ok) return;

            const data = await response.json() as Station;
            setStation(data);
        }

        fetchStation();
    }, []);

    const [journeys, setJourneys] = useState<Journey[]>([]);
    const journeysRef = useRef<Journey[]>([]);

    const [scheduled, setScheduled] = useState<Connection[]>([]);

    const [startDate, setStartDate] = useState<Date>(() => {
        const date: Date = new Date();
        date.setSeconds(0);
        return date;
    });
    const [endDate, setEndDate] = useState<Date>();

    const calculateDuration = (): number => {
        if (!endDate) return 60;
        return (endDate.getTime() - startDate.getTime()) / 60000;
    }

    const updateJourneys = async () => {
        const request = await fetch(`/api/v1/station/departures?id=${station?.evaNr}&when=${startDate.toISOString()}&duration=${calculateDuration()}&results=1000`);
        if (!request.ok) return;

        const response = await request.json();
        if (!Array.isArray(response.entries)) return;

        const connections: Connection[] = response.entries as Connection[];

        const prevJourneys = journeysRef.current;

        const mappedJourneys = mapConnections(prevJourneys, connections);
        const sorted = sort(mappedJourneys.journeys);

        setJourneys(sorted);
        journeysRef.current = sorted;

        // TODO: remove scheduled
        setScheduled((currentTrips: Connection[]) => {
            const tripMap = new Map(currentTrips.map(trip => [trip.tripId, trip]));
            connections.forEach((incomingTrip) => {
                if (tripMap.has(incomingTrip.tripId))
                    tripMap.set(incomingTrip.tripId, {...tripMap.get(incomingTrip.tripId), ...incomingTrip});
                else tripMap.set(incomingTrip.tripId, incomingTrip);
            });

            return Array.from(tripMap.values());
        });
    }

    useEffect(() => {
        // update end date
        setEndDate((_) => {
            const date = new Date();
            date.setHours(startDate.getHours() + 1);
            return date;
        });

        const intervalId = setInterval(updateJourneys, 15 * 1000);
        return () => clearInterval(intervalId);
    }, []);

    return (
        <div className="h-screen flex flex-col overflow-hidden md:space-y-4">
            <Navbar id={station?.evaNr ?? ""}/>

            <div className="container mx-auto flex justify-between items-center px-4">
                <span className="text-xl md:text-4xl font-semibold mt-4 px-2 md:px-4">{station?.name ?? ""}</span>
                <Clock className="text-2xl md:text-4xl font-medium mt-4 px-2 md:px-4"/>
            </div>

            <ScheduledHeader isDeparture={true}/>
            <div className="container mx-auto flex-grow overflow-y-auto scrollbar-hidden">
                {scheduled.length > 0 && scheduled.map((item: Connection, index: number) => (
                    <ScheduledComponent
                        key={item.tripId}
                        connection={item}
                        isDeparture={true}
                        isEven={index % 2 === 0}
                    />
                ))}
            </div>
        </div>
    )
}
