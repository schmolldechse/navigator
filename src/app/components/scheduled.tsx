import {ScheduledLine} from "@/app/lib/schedule";
import {useEffect, useState} from "react";

interface ScheduledProps {
    scheduled: ScheduledLine,
    isEven: boolean
}

const ScheduledComponent: React.FC<ScheduledProps> = ({scheduled, isEven}) => {
    const [color, setColor] = useState<any>();
    const backgroundColor = isEven ? "#0a0a0a" : "#1a1a1a";

    useEffect(() => {
        const fetchColor = async () => {
            const colorRequest = await fetch(`/api/v1/color?name=${scheduled.line.name}`, {method: 'GET'});
            if (!colorRequest.ok) return;

            const colorData = await colorRequest.json();
            setColor(colorData);
        }

        fetchColor();
    }, []);

    const isDelayed = () => {
        const planned = new Date(scheduled.plannedWhen);
        const actual = new Date(scheduled.actualWhen);
        return actual > planned;
    }

    return (
        <div className="container mx-auto flex justify-between space-x-4 text-white text-[28px] font-bold border-gray-400 pb-4"
             style={{backgroundColor}}
        >
            {/* First col */}
            <div className={`flex-[1] text-right mr-8 border-t pt-4 px-2 space-y-4`}>
                {/* Line */}
                <span className={`${color ? 'p-2 rounded-md' : ''} text-base`}
                      style={{backgroundColor: color?.backgroundColor || 'inherit'}}
                >
                        {scheduled.line.name}
                </span>

                {/* Departure time */}
                <div className="flex flex-row items-center justify-end space-x-2">
                    <span
                        className={`${isDelayed() ? '' : ''} flex items-center justify-center`}
                        style={{height: '2.5rem', padding: '0'}}
                    >
                        {new Date(scheduled.plannedWhen).toLocaleTimeString([], {hour: '2-digit', minute: '2-digit'})}
                    </span>
                    {isDelayed() ? (
                        <span className="bg-[#ededed] text-[#0a0a0a] flex items-center justify-center text-[20px]"
                              style={{height: '1.75rem', padding: '0 0.5rem'}}
                        >
                            {new Date(scheduled.actualWhen).toLocaleTimeString([], {hour: '2-digit', minute: '2-digit'})}
                        </span>
                    ) : (<></>)}
                </div>
            </div>

            {/* Second col */}
            <div className="flex-[4] text-left border-t pt-4 px-2">
                <p>Über</p>
                <p>Ziel</p>
            </div>

            {/* Third col */}
            <div className="flex-[1] flex justify-end items-end text-right border-t pt-4 text-3xl">
                {scheduled.plannedPlatform === scheduled.actualPlatform ? (
                    <span>{scheduled.plannedPlatform}</span>
                ) : (
                    <span className="bg-[#ededed] text-[#0a0a0a] w-full p-2">{scheduled.actualPlatform}</span>
                )}
            </div>
        </div>
    );
}

export default ScheduledComponent;
