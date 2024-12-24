import {useEffect, useState} from "react";
import {Trip} from "@/app/lib/trip";

interface ScheduledProps {
    trip: Trip,
    isDeparture: boolean,
    isEven: boolean
}

const ScheduledComponent: React.FC<ScheduledProps> = ({trip, isDeparture, isEven}) => {
    const [color, setColor] = useState<any>();
    const backgroundColor = isEven ? "#0a0a0a" : "#1a1a1a";

    console.log(trip);

    useEffect(() => {
        const fetchColor = async () => {
            const colorRequest = await fetch(`/api/v1/color?id=${trip.lineInformation.id}`, {method: 'GET'});
            if (!colorRequest.ok) return;

            const colorData = await colorRequest.json();
            setColor(colorData);
        }

        fetchColor();
    }, []);

    const isDelayed = () => {
        const planned = new Date(isDeparture ? trip.departure.plannedTime : trip.arrival.plannedTime);
        const actual = new Date(isDeparture ? trip.departure.actualTime : trip.arrival.actualTime);

        const difference = (actual.getTime() - planned.getTime()) / 60000; // in min
        return difference >= 1;
    }

    const showPlatform = () => {
        return (
            <>
                {isDeparture ? (
                    <>
                        {trip.departure.plannedPlatform === trip.departure.actualPlatform ? (
                            <span>{trip.departure.plannedPlatform}</span>
                        ) : (
                            <span className="bg-[#ededed] text-[#0a0a0a] w-full p-2">{trip.departure.actualPlatform}</span>
                        )}
                    </>
                ) : (
                    <>
                        {trip.arrival.plannedPlatform === trip.arrival.actualPlatform ? (
                            <span>{trip.arrival.plannedPlatform}</span>
                        ) : (
                            <span className="bg-[#ededed] text-[#0a0a0a] w-full p-2">{trip.arrival.actualPlatform}</span>
                        )}
                    </>
                )}
            </>
        )
    }

    return (
        <div className={`container mx-auto flex justify-between space-x-4 text-[28px] font-medium ${trip.cancelled ? 'border-[#0a0a0a] text-black' : 'border-gray-400' } pb-4`}
             style={{ backgroundColor: trip.cancelled ? '#ededed' : backgroundColor }}
        >
            {/* First col */}
            <div className={`flex-[1] text-right mr-8 border-t pt-4 space-y-4`}>
                {/* Line */}
                <span className={`${color ? 'p-2 rounded-2xl px-4 font-bold' : ''} text-xl`}
                      style={{backgroundColor: color?.backgroundColor || 'inherit'}}
                >
                        {trip.lineInformation.fullName}
                </span>

                {/* Departure time */}
                <div className="flex flex-row items-center justify-end space-x-2">
                    <span
                        className={`${isDelayed() ? '' : ''} flex items-center justify-center`}
                        style={{height: '2rem'}}
                    >
                        {new Date(isDeparture ? trip.departure.plannedTime : trip.arrival.plannedTime).toLocaleTimeString([], {
                            hour: '2-digit',
                            minute: '2-digit'
                        })}
                    </span>
                    {isDelayed() ? (
                        <span
                            className={`${isDelayed() ? 'font-bold' : ''} bg-[#ededed] text-[#0a0a0a] flex items-center justify-center text-[20px]`}
                            style={{height: '1.5rem', padding: '0 0.4rem'}}
                        >
                            {new Date(isDeparture ? trip.departure.actualTime : trip.arrival.actualTime).toLocaleTimeString([], {
                                hour: '2-digit',
                                minute: '2-digit'
                            })}
                        </span>
                    ) : (<></>)}
                </div>
            </div>

            {/* Second col */}
            <div className="flex-[4] flex items-end text-left border-t pt-4 space-y-4">
                <span>{trip.destination.name}</span>
            </div>

            {/* Third col */}
            <div className="flex-[1] flex justify-end items-end text-right border-t pt-4 pr-1 text-3xl">
                {showPlatform()}
            </div>
        </div>
    );
}

export default ScheduledComponent;
