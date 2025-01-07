'use client';

import {useParams} from "next/navigation";
import {useEffect, useRef, useState} from "react";
import Navbar from "@/app/components/navbar";
import Clock from "@/app/components/clock";
import ScheduledComponent from "@/app/components/scheduled";
import ScheduledHeader from "@/app/components/scheduled-header";
import {Connection, Journey} from "@/app/lib/objects";
import {mapConnections, sort} from "@/app/lib/mapper";

export default function Departures() {
    const params = useParams();
    const [station, setStation] = useState<{ id: string; name?: string | undefined }>({
        id: Array.isArray(params.id) ? params.id[0] : params.id || "",
        name: undefined
    });

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

    // obtain language for results
    const browserLanguage = (): string => {
        const supported = ['en', 'de'];
        const language = navigator.language.split("-")[0];
        return supported.includes(language) ? language : 'en';
    }

    // fetch once
    const journeysFromDB = async (): Promise<Journey[]> => {
        const request = await fetch(`/api/v1/bahnhof-proxy?id=${station.id}&type=departures&duration=60&locale=${browserLanguage()}`);
        if (!request.ok) return;

        const response = await request.json();
        if (!response.entries || !Array.isArray(response.entries)) return;

        return response.entries as Journey[];
    }

    const updateJourneys = async () => {
        const request = await fetch(`/api/v1/station/departures?id=${station.id}&when=${startDate.toISOString()}&duration=${calculateDuration()}&results=1000`);
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

        // fetch station name from HAFAS
        const fetchStationName = async () => {
            const response = await fetch(`/api/v1/station/`, {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({id: station.id})
            });
            if (!response.ok) return;

            const data = await response.json();
            setStation((prev) => ({...prev, name: data.station}));
        }
        fetchStationName();

        // fetch journeys once from Bahnhof API and update them every 15s
        const initJourneys = async () => {
            const fetchedJourneys: Journey[] = await journeysFromDB();
            if (fetchedJourneys.length === 0) return;

            journeysRef.current = fetchedJourneys;

            updateJourneys(fetchedJourneys);
        }
        initJourneys();

        const intervalId = setInterval(updateJourneys, 15 * 1000);
        return () => clearInterval(intervalId);
    }, []);

    return (
        <div className="h-screen flex flex-col overflow-hidden md:space-y-4">
            <Navbar id={station.id}/>

            <div className="container mx-auto flex justify-between items-center px-4">
                <span className="text-xl md:text-4xl font-semibold mt-4 px-2 md:px-4">{station.name}</span>
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
