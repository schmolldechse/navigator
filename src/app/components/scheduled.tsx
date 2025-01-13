import {useState} from "react";
import {Connection} from "@/app/lib/objects";

interface ScheduledProps {
    connection: Connection,
    isDeparture: boolean,
}

const ScheduledComponent: React.FC<ScheduledProps> = ({connection, isDeparture}) => {
    const [color, setColor] = useState<any>();

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
        const platform = isDeparture ? connection?.departure : connection.arrival;
        const isSame = platform.plannedPlatform === platform.actualPlatform;

        return (
            <span className={`md:w-full ${isSame ? "" : "bg-[#ededed] text-black px-2 md:px-0"}`}>
                {isSame ? platform.plannedPlatform : platform.actualPlatform}
            </span>
        );
    }

    return (
        <div
            className={`border-gray-40 ${!connection?.cancelled ? '' : 'text-[#0a0a0a] bg-[#ededed]'}`}
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
            <span
                className={`mx-auto hidden md:flex justify-between text-[30px] font-medium ${connection?.cancelled ? 'bg-[#ededed] text-black' : ''}`}>
                {/* first col */}
                <span className="flex-[1] flex flex-col items-end mr-8 border-t">
                    {/* line nr*/}
                    <span
                        className={`text-xl ${!color ? 'font-semibold' : 'p-2 rounded-2xl px-4 font-bold'}`}>{connection?.lineInformation!!.fullName}
                    </span>

                    {/* time information */}
                    <div className={"flex items-center space-x-2"}>
                        <span>{new Date(isDeparture ? connection?.departure!!.plannedTime : connection?.arrival.plannedTime!!).toLocaleTimeString("de-DE", {
                            hour: '2-digit',
                            minute: '2-digit'
                        })}
                        </span>
                        {isDelayed() && (
                            <span
                                className={"text-[20px] px-[0.4rem] font-bold bg-[#ededed] text-[#0a0a0a]"}>{new Date(isDeparture ? connection?.departure!!.actualTime : connection?.arrival.actualTime!!).toLocaleTimeString("de-DE", {
                                hour: '2-digit',
                                minute: '2-digit'
                            })}
                            </span>
                        )}
                    </div>
                </span>

                {/* second col */}
                <span className="flex-[4] flex items-end mr-4 border-t">
                    {isDeparture ? connection?.destination!.name : connection?.origin!.name}
                </span>

                {/* third col */}
                <span className="flex-[1] flex justify-end items-end text-right border-t">
                    {showPlatform()}
                </span>
            </span>
        </div>
    );
}

export default ScheduledComponent;
