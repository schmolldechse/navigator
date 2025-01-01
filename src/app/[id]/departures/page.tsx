'use client';

import {useParams} from "next/navigation";
import {useEffect, useState} from "react";
import Navbar from "@/app/components/navbar";
import Clock from "@/app/components/clock";
import ScheduledComponent from "@/app/components/scheduled";
import ScheduledHeader from "@/app/components/scheduled-header";
import {Trip} from "@/app/lib/trip";

export default function Departures() {
    const params = useParams();
    const [station, setStation] = useState<{ id: string; name?: string | undefined }>({
        id: Array.isArray(params.id) ? params.id[0] : params.id || "",
        name: undefined
    });

    const [scheduled, setScheduled] = useState<Trip[]>([]);

    const [startDate, setStartDate] = useState<Date>(new Date());
    const [endDate, setEndDate] = useState<Date>();

    const calculateDuration = (): number => {
        if (!endDate) return 60;
        return (endDate.getTime() - startDate.getTime()) / 60000;
    }

    const fetchTrips = async () => {
        const request = await fetch(`/api/v1/station/departures?id=${station.id}&when=${startDate.toISOString()}&duration=${calculateDuration()}&results=1000`);
        if (!request.ok) return;

        const response = await request.json();
        if (!response.success || response.entries.length === 0) return;

        const trips: Trip[] = response.entries as Trip[];
        setScheduled((currentTrips: Trip[]) => {
            const tripMap = new Map(currentTrips.map(trip => [trip.tripId, trip]));
            trips.forEach((incomingTrip) => {
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

        // fetch trips from HAFAS once & update them every 15s
        fetchTrips();

        const intervalId = setInterval(fetchTrips, 15 * 1000);
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
                {scheduled.length > 0 && scheduled.map((item: Trip, index: number) => (
                    <ScheduledComponent
                        key={item.tripId}
                        trip={item}
                        isDeparture={true}
                        isEven={index % 2 === 0}
                    />
                ))}
            </div>
        </div>
    )
}
