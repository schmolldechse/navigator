'use client';

import {useEffect, useState} from "react";

interface ClockProps {
    className: string
}

const Clock: React.FC<ClockProps> = ({ className }) => {
    const [currentTime, setCurrentTime] = useState<string>("00:00");
    const [showColon, setShowColon] = useState<boolean>(true);

    useEffect(() => {
        const timer = setInterval(() => {
            const now = new Date();
            const hours = String(now.getHours()).padStart(2, '0');
            const minutes = String(now.getMinutes()).padStart(2, '0');
            setCurrentTime(`${hours}${showColon ? ':' : ' '}${minutes}`);
            setShowColon((prev) => !prev);
        }, 1000);

        return () => clearInterval(timer); // Cleanup interval on unmount
    }, [showColon]);

    return (
        <span className={className}>{currentTime}</span>
    );
}

export default Clock;
