import {useEffect, useState} from "react";
import {Connection} from "@/app/lib/objects";

interface ScheduledProps {
    connection: Connection,
    isDeparture: boolean,
    isEven: boolean
}

const ScheduledComponent: React.FC<ScheduledProps> = ({connection, isDeparture, isEven}) => {
    const [color, setColor] = useState<any>();
    const backgroundColor = isEven ? "#0a0a0a" : "#1a1a1a";

    // TODO: not possible to find out at the moment
    /**
    useEffect(() => {
        const fetchColor = async () => {
            const colorRequest = await fetch(`/api/v1/color?id=${connection.lineInformation.id}`, {method: 'GET'});
            if (!colorRequest.ok) return;

            const colorData = await colorRequest.json();
            if (!colorData.success) return;
            setColor(colorData.entry);
        }

        fetchColor();
    }, []);
    */

    const isDelayed = () => {
        const planned = new Date(isDeparture ? connection.departure.plannedTime : connection.arrival.plannedTime);
        const actual = new Date(isDeparture ? connection.departure.actualTime : connection.arrival.actualTime);

        const difference = (actual.getTime() - planned.getTime()) / 60000; // in min
        return difference >= 1;
    }

    const showPlatform = () => {
        return (<>
            {isDeparture ? (<>
                {connection.departure.plannedPlatform === connection.departure.actualPlatform ? (
                    <span>{connection.departure.plannedPlatform}</span>
                ) : (
                    <span className="bg-[#ededed] md:w-full">{connection.departure.actualPlatform}</span>
                )}
            </>) : (<>
                {connection.arrival.plannedPlatform === connection.arrival.actualPlatform ? (
                    <span>{connection.arrival.plannedPlatform}</span>
                ) : (
                    <span className="bg-[#ededed] md:w-full">{connection.arrival.actualPlatform}</span>
                )}
            </>)}
        </>)
    }

    return (
        <div
            className={`border-gray-40 ${!connection?.cancelled ? '' : 'text-[#0a0a0a]' }`}
            style={{ backgroundColor: connection.cancelled ? "#ededed" : backgroundColor }}
        >
            {/* layout for smaller screens (under md) */}
            <div
                className={`p-2 md:hidden border-t space-y-2 font-medium`}
            >
                {/* first line */}
                <span className={`${color ? 'py-[0.2rem] px-[0.8rem] rounded-xl' : ''} text-lg font-bold`}
                      style={{backgroundColor: color ? `${color.backgroundColor}` : ''}}
                >
                    {connection.lineInformation?.fullName}
                </span>

                {/* second line */}
                <div className="flex flex-row items-center font-semibold">
                    {/* scheduled time */}
                    <div className="flex-[1] flex flex-row items-center space-x-2 text-2xl">
                        <span>
                            {new Date(isDeparture ? connection.departure.plannedTime : connection.arrival.plannedTime).toLocaleTimeString("de-DE", {
                                hour: '2-digit',
                                minute: '2-digit'
                            })}
                        </span>
                        {isDelayed() && (
                            <span
                                className={`bg-[#ededed] text-[#0a0a0a] text-sm h-full px-[0.3rem] py-[0.05rem]`}
                            >
                                {new Date(isDeparture ? connection.departure.actualTime : connection.arrival.actualTime).toLocaleTimeString("de-DE", {
                                    hour: '2-digit',
                                    minute: '2-digit'
                                })}
                            </span>
                        )}
                    </div>

                    {/* track */}
                    <span className="text-right text-2xl">{showPlatform()}</span>
                </div>

                {/* third line */}
                <span className="text-2xl">{isDeparture ? connection.destination.name : connection.origin.name}</span>
            </div>

            {/* layout for greater screens (above md) */}
            <div
                className={`container mx-auto hidden md:flex justify-between space-x-4 text-[28px] font-medium ${connection.cancelled ? 'text-black' : ''} pb-4`}
            >
                {/* First col */}
                <div className={`flex-[1] text-right mr-8 border-t pt-4 space-y-4`}>
                    {/* Line */}
                    <span className={`${color ? 'p-2 rounded-2xl px-4 font-bold' : ''} text-xl`}
                          style={{backgroundColor: color?.backgroundColor || 'inherit'}}
                    >
                        {connection.lineInformation?.fullName}
                    </span>

                    {/* Departure time */}
                    <div className="flex flex-row items-center justify-end space-x-2">
                        <span
                            className={`${isDelayed() ? '' : ''} flex items-center justify-center`}
                            style={{height: '2rem'}}
                        >
                            {new Date(isDeparture ? connection.departure.plannedTime : connection.arrival.plannedTime).toLocaleTimeString("de-DE", {
                                hour: '2-digit',
                                minute: '2-digit'
                            })}
                        </span>
                        {isDelayed() && (
                            <span
                                className={`${isDelayed() ? 'font-bold' : ''} bg-[#ededed] text-[#0a0a0a] flex items-center justify-center text-[20px]`}
                                style={{height: '1.5rem', padding: '0 0.4rem'}}
                            >
                                {new Date(isDeparture ? connection.departure.actualTime : connection.arrival.actualTime).toLocaleTimeString("de-DE", {
                                    hour: '2-digit',
                                    minute: '2-digit'
                                })}
                            </span>
                        )}
                    </div>
                </div>

                {/* Second col */}
                <div className="flex-[4] flex items-end text-left border-t pt-4">
                    <span>{isDeparture ? connection.destination.name : connection.origin.name}</span>
                </div>

                {/* Third col */}
                <div className="flex-[1] flex justify-end items-end text-right border-t pt-4 text-3xl">
                    {showPlatform()}
                </div>
            </div>
        </div>
    );
}

export default ScheduledComponent;
