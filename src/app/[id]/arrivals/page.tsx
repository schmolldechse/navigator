'use client';

import {useParams} from "next/navigation";
import {useEffect, useState} from "react";
import Navbar from "@/app/components/navbar";
import Clock from "@/app/components/clock";
import ScheduledComponent from "@/app/components/scheduled";
import ScheduledHeader from "@/app/components/scheduled-header";
import {Trip} from "@/app/lib/trip";

export default function Arrivals() {
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

    // fetch departures from HAFAS-v1
    const arrivalsV1 = async (date: Date): Promise<Trip[]> => {
        const response = await fetch(`https://hafas-v1.voldechse.wtf/stops/${station.id}/arrivals?when=${date.toISOString()}&duration=${calculateDuration()}&results=1000`, {method: 'GET'});
        if (!response.ok) return [];

        const data = await response.json();
        if (!data?.arrivals || !Array.isArray(data.arrivals)) return [];

        const map = new Map<string, Trip>();
        data.arrivals.forEach((arrival: any) => {
            const tripId = arrival.tripId;
            if (!tripId || map.has(tripId)) return;

            const trip: Trip = {
                tripId,
                origin: {
                    id: arrival.origin.id,
                    name: arrival.origin.name
                },
                arrival: {
                    plannedTime: arrival.plannedWhen,
                    actualTime: arrival.when,                     // nullable
                    delay: arrival.delay,                         // nullable
                    plannedPlatform: arrival.plannedPlatform,     // nullable
                    actualPlatform: arrival.platform
                },
                lineInformation: {
                    productName: arrival.line.productName,
                    fullName: arrival.line.name,
                    id: arrival.line.id,
                    fahrtNr: arrival.line.fahrtNr,
                    operator: {
                        id: arrival.line.operator?.id || '',
                        name: arrival.line.operator?.name || ''
                    }
                },
                remarks: arrival.remarks,
                cancelled: arrival.cancelled || false
            }

            map.set(tripId, trip);
        });

        return Array.from(map.values());
    }

    const arrivalsV2 = async (date: Date, trips: Trip[]): Promise<Trip[]> => {
        const response = await fetch(`https://hafas-v2.voldechse.wtf/stops/${station.id}/arrivals?when=${date.toISOString()}&duration=${calculateDuration()}&results=1000`, {method: 'GET'});
        if (!response.ok) return trips;

        const data = await response.json();
        if (!data?.arrivals || !Array.isArray(data.arrivals)) return trips;

        return trips.map((trip: Trip) => {
            const matchingArrival = data.arrivals.find((arrival: any) => {
                return (
                    arrival.plannedWhen === trip.arrival.plannedTime &&
                    arrival.line?.fahrtNr === trip.lineInformation?.fahrtNr
                );
            });

            if (matchingArrival) {
                return {
                    ...trip,
                    origin: {
                        id: matchingArrival.origin.id,
                        name: matchingArrival.origin.name
                    },
                    arrival: {
                        ...trip.arrival,
                        plannedTime: matchingArrival.plannedWhen,
                        actualTime: matchingArrival.when,
                        delay: matchingArrival.delay,
                        plannedPlatform: matchingArrival.plannedPlatform,
                        actualPlatform: matchingArrival.platform
                    }
                };
            }

            return trip;
        });
    }

    const updateScheduledTrips = (newTrips: Trip[]) => {
        setScheduled((currentTrips) => {
            const tripMap = new Map(currentTrips.map(trip => [trip.tripId, trip]));
            newTrips.forEach((newTrip) => {
                if (tripMap.has(newTrip.tripId))
                    tripMap.set(newTrip.tripId, {...tripMap.get(newTrip.tripId), ...newTrip});
                else tripMap.set(newTrip.tripId, newTrip);
            });

            return Array.from(tripMap.values()).sort((a, b) =>
                new Date(a.arrival.actualTime || a.arrival.plannedTime).getTime() -
                new Date(b.arrival.actualTime || b.arrival.plannedTime).getTime()
            );
        });
    }

    const fetchTrips = async () => {
        const v2Trips = await arrivalsV1(startDate);
        if (!v2Trips) return;

        const updatedTrips = await arrivalsV2(startDate, v2Trips);
        if (!updatedTrips) return;

        updateScheduledTrips(updatedTrips);
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
        <div className="h-screen flex flex-col overflow-hidden space-y-4">
            <Navbar id={station.id}/>

            <div className="container mx-auto flex justify-between items-center px-4">
                <span className="text-4xl font-semibold mt-4 px-4">{station.name}</span>
                <Clock className="text-4xl font-medium mt-4 px-4"/>
            </div>

            <ScheduledHeader isDeparture={false}/>
            <div className="container mx-auto flex-grow overflow-y-auto scrollbar-hidden">
                {scheduled.length > 0 ? (
                    scheduled.map((item: Trip, index: number) => (
                        <ScheduledComponent
                            key={item.tripId}
                            trip={item}
                            isDeparture={false}
                            isEven={index % 2 === 0}
                        />
                    ))
                ) : <></>}
            </div>
        </div>
    )
}
