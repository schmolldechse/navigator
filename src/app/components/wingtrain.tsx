import React, { useEffect, useRef } from "react";
import { Journey } from "@/app/lib/objects";
import ScheduledComponent from "@/app/components/scheduled";

interface Props {
    journey: Journey;
    isDeparture: boolean;
}

const WingTrain: React.FC<Props> = ({ journey, isDeparture }) => {
    return (
        <div className="flex flex-col">
            {journey.connections.map((connection, index) => (
                <div key={`connection${index}`}>
                    <ScheduledComponent
                        connection={connection}
                        isDeparture={isDeparture}
                        renderBorder={index === 0} // first renders border
                        renderTime={index === journey.connections.length - 1} // last renders time
                    />
                </div>
            ))}
        </div>
    );
};

export default WingTrain;
