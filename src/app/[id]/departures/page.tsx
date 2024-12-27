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

    // fetch departures from HAFAS-v1
    const departuresV1 = async (date: Date): Promise<Trip[]> => {
        const response = await fetch(`https://hafas-v1.voldechse.wtf/stops/${station.id}/departures?when=${date.toISOString()}&duration=${calculateDuration()}&results=1000`, {method: 'GET'});
        if (!response.ok) return [];

        const data = await response.json();
        if (!data?.departures || !Array.isArray(data.departures)) return [];

        const map = new Map<string, Trip>();
        data.departures.forEach((departure: any) => {
            const tripId = departure.tripId;
            if (!tripId || map.has(tripId)) return;

            const trip: Trip = {
                tripId,
                destination: {
                    id: departure.destination.id,
                    name: departure.destination.name
                },
                departure: {
                    plannedTime: departure.plannedWhen,
                    actualTime: departure.when,                     // nullable
                    delay: departure.delay,                         // nullable
                    plannedPlatform: departure.plannedPlatform,     // nullable
                    actualPlatform: departure.platform
                },
                lineInformation: {
                    productName: departure.line.productName,
                    fullName: departure.line.name,
                    id: departure.line.id,
                    fahrtNr: departure.line.fahrtNr,
                    operator: {
                        id: departure.line.operator?.id || '',
                        name: departure.line.operator?.name || ''
                    }
                },
                remarks: departure.remarks,
                cancelled: departure.cancelled || false
            }

            map.set(tripId, trip);
        });

        return Array.from(map.values());
    }

    const departuresV2 = async (date: Date, trips: Trip[]): Promise<Trip[]> => {
        const response = await fetch(`https://hafas-v2.voldechse.wtf/stops/${station.id}/departures?when=${date.toISOString()}&duration=${calculateDuration()}&results=1000`, {method: 'GET'});
        if (!response.ok) return trips;

        const data = await response.json();
        if (!data?.departures || !Array.isArray(data.departures)) return trips;

        return trips.map((trip: Trip) => {
            const matchingDeparture = data.departures.find((departure: any) => {
                return (
                    departure.plannedWhen === trip.departure.plannedTime &&
                    departure.line?.fahrtNr === trip.lineInformation?.fahrtNr
                );
            });

            if (matchingDeparture) {
                return {
                    ...trip,
                    departure: {
                        ...trip.departure,
                        plannedTime: matchingDeparture.plannedWhen,
                        actualTime: matchingDeparture.when,
                        delay: matchingDeparture.delay,
                        plannedPlatform: matchingDeparture.plannedPlatform,
                        actualPlatform: matchingDeparture.platform
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
                new Date(a.departure.actualTime || a.departure.plannedTime).getTime() -
                new Date(b.departure.actualTime || b.departure.plannedTime).getTime()
            );
        });
    }

    const fetchTrips = async () => {
        const v1Trips = await departuresV1(startDate);
        if (!v1Trips) return;

        const updatedTrips = await departuresV2(startDate, v1Trips);
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
            const response = await fetch(`/api/v1/search/`, {
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
                {scheduled.length > 0 ? (
                    scheduled.map((item: Trip, index: number) => (
                        <ScheduledComponent
                            key={item.tripId}
                            trip={item}
                            isDeparture={true}
                            isEven={index % 2 === 0}
                        />
                    ))
                ) : <></>}
            </div>
        </div>
    )
}
