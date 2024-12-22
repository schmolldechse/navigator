'use client';

import {ScheduledLine} from "@/app/lib/schedule";

interface ScheduledProps {
    scheduled: ScheduledLine,
    isEven: boolean
}

const ScheduledComponent: React.FC<ScheduledProps> = ({ scheduled, isEven }) => {
    const backgroundColor = isEven ? "#0a0a0a" : "#1a1a1a";

    return (
        <div
            className="p-4 rounded-md text-white hover:cursor-pointer"
            style={{ backgroundColor }}
        >
            <p className="font-semibold">{scheduled.line.name}</p>
            <p className="text-sm">
                Richtung: {scheduled.directionName} - Abfahrt: {scheduled.plannedWhen}
            </p>
        </div>
    );
}

export default ScheduledComponent;
