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
        if (!response.ok) return;

        const data = await response.json();
        if (!data?.departures) return;
        if (!Array.isArray(data.departures)) return;

        return data.departures.filter((departure: any) => departure.tripId)
            .map((departure: any) => {
                const trip: Trip = {
                    tripId: departure.tripId,
                    destination: {
                        id: departure.destination.id,
                        name: departure.destination.name,
                    },
                    departure: {
                        plannedTime: departure.plannedWhen,
                        actualTime: departure.when,
                        delay: departure.delay,
                        plannedPlatform: departure.plannedPlatform,
                        actualPlatform: departure.platform
                    },
                    lineInformation: {
                        productName: departure.line.productName,
                        fullName: departure.line.name,
                        id: departure.line.id,
                        fahrtNr: departure.line.fahrtNr,
                        operator: {
                            id: departure.line.operator.id,
                            name: departure.line.operator.name,
                        },
                    },
                };

                return trip;
            });
    }

    const departuresV2 = async (date: Date): Promise<Trip[]> => {
        const response = await fetch(`https://hafas-v2.voldechse.wtf/stops/${station.id}/departures?when=${date.toISOString()}&duration=${calculateDuration()}&results=1000`, {method: 'GET'});
        if (!response.ok) return;

        const data = await response.json();
        if (!data?.departures) return;
        if (!Array.isArray(data.departures)) return;

        return data.departures.filter((departure: any) => departure.tripId)
            .map((departure: any) => {
                const trip: Trip = {
                    tripId: departure.tripId,
                    lineInformation: {
                        productName: departure.line.productName,
                        fullName: departure.line.name,
                        id: departure.line.id,
                        fahrtNr: departure.line.fahrtNr
                    },
                    departure: {
                        plannedTime: departure.plannedWhen,
                        actualTime: departure.when,
                        delay: `${departure.delay || 0}`,
                        plannedPlatform: departure.plannedPlatform || '',
                        actualPlatform: departure.platform || '',
                    },
                };

                return trip;
            });
    }

    const updateTrips = async (date: Date): Promise<Trip[]> => {
        const tripsV1 = await departuresV1(date);
        const tripsV2 = await departuresV2(date);

        if (!tripsV1 || !tripsV2) return [];

        const tripsV2Map = new Map<string, Trip>(
            tripsV2.map((v2Trip) => [
                `${v2Trip.departure?.plannedTime}-${v2Trip.lineInformation?.fahrtNr}`,
                v2Trip,
            ])
        );

        return tripsV1.map((v1Trip: Trip) => {
            const key = `${v1Trip.departure?.plannedTime}-${v1Trip.lineInformation?.fahrtNr}`;
            const matchingV2Trip = tripsV2Map.get(key);

            if (matchingV2Trip) {
                // Aktualisiere die departure-Daten
                v1Trip.departure = {
                    ...v1Trip.departure,
                    plannedTime: matchingV2Trip.departure?.plannedTime || v1Trip.departure?.plannedTime,
                    actualTime: matchingV2Trip.departure?.actualTime || '',
                    delay: matchingV2Trip.departure?.delay || '0',
                    plannedPlatform: matchingV2Trip.departure?.plannedPlatform || '',
                    actualPlatform: matchingV2Trip.departure?.actualPlatform || '',
                };
            }

            return v1Trip;
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
            const response = await fetch(`https://v6.db.transport.rest/stations/${station.id}`, {method: 'GET'});
            if (!response.ok) return;

            const data = await response.json();
            setStation((prev) => ({...prev, name: data.name}));
        }
        fetchStationName();

        // fetch trips
        const fetchTrips = async () => {
            const trips = await updateTrips(startDate);
            if (!trips) return;

            const newTrips = trips.filter((newTrip: Trip) => !scheduled.some((existingTrip: Trip) => existingTrip.tripId === newTrip.tripId));
            setScheduled((prev) => [...prev, ...newTrips]);
        }
        fetchTrips();
    }, []);

    return (
        <div className="h-screen flex flex-col overflow-hidden space-y-4">
            <Navbar id={station.id}/>

            <div className="container mx-auto flex justify-between items-center px-4">
                <span className="text-4xl font-semibold mt-4 px-4">{station.name}</span>
                <Clock className="text-4xl font-medium mt-4 px-4"/>
            </div>

            <ScheduledHeader/>
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
