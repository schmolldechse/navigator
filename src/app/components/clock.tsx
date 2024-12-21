'use client';

import {useEffect, useState} from "react";

interface ClockProps {
    className: string
}

const Clock: React.FC<ClockProps> = ({ className }) => {
    const [hours, setHours] = useState<string>("00");
    const [minutes, setMinutes] = useState<string>("00");

    const [showColon, setShowColon] = useState<boolean>(true);

    useEffect(() => {
        const updateTime = () => {
            const now = new Date();
            setHours(String(now.getHours()).padStart(2, '0'));
            setMinutes(String(now.getMinutes()).padStart(2, '0'));
        }

        updateTime();

        const timer = setInterval(() => {
            updateTime();
            setShowColon((prev) => !prev);
        }, 1000);

        return () => clearInterval(timer);
    }, []);

    return (
        <span className={className}>
            {hours}
            <span className={`${showColon ? '' : 'invisible'}`}>:</span>
            {minutes}
        </span>
    );
}

export default Clock;
