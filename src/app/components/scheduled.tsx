'use client';

import {ScheduledLine} from "@/app/lib/schedule";

interface ScheduledProps {
    scheduled: ScheduledLine,
    isEven: boolean
}

const ScheduledComponent: React.FC<ScheduledProps> = ({scheduled, isEven}) => {
    const backgroundColor = isEven ? "#0a0a0a" : "#1a1a1a";

    const isDelayed = () => {
        const planned = new Date(scheduled.plannedWhen);
        const actual = new Date(scheduled.actualWhen);
        return actual > planned;
    }

    return (
        <div className="container mx-auto flex justify-between space-x-4 text-white text-base font-bold border-gray-400 pb-4"
             style={{backgroundColor}}
        >
            {/* First col */}
            <div className={`flex-[1] text-right mr-8 border-t pt-4 px-2 space-y-4`}>
                {/* Line */}
                <span className={`${scheduled.color ? 'p-2 rounded-md' : ''}`}
                      style={{backgroundColor: scheduled.color?.backgroundColor || 'inherit'}}
                >
                        {scheduled.line.name}
                </span>

                {/* Departure time */}
                <div className="flex flex-row items-center justify-end space-x-4 text-xl">
                    <span className={`${isDelayed() ? 'line-through text-red-500' : ''}`}>
                        {new Date(scheduled.plannedWhen).toLocaleTimeString([], {hour: '2-digit', minute: '2-digit'})}
                    </span>
                    {isDelayed() ? (
                        <span className="bg-red-500 p-1">
                            {new Date(scheduled.actualWhen).toLocaleTimeString([], {hour: '2-digit', minute: '2-digit'})}
                        </span>
                    ) : (<></>)}
                </div>
            </div>

            {/* Second col */}
            <div className="flex-[4] text-left border-t pt-4">
                <p>Über</p>
                <p>Ziel</p>
            </div>

            {/* Third col */}
            <div className="flex-[1] flex justify-end items-end text-right border-t pt-4">
                <p>Gleis</p>
            </div>
        </div>
    );
}

export default ScheduledComponent;
