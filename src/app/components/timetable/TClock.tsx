'use client';

import { DateTime } from "luxon";
import {useEffect, useState} from "react";

interface Props {
    className: string
}

const TClock: React.FC<Props> = ({ className }) => {
    const [time, setTime] = useState<DateTime>(DateTime.local());
    const [showColon, setShowColon] = useState<boolean>(true);

    useEffect(() => {
        const updateTime = () => setTime(DateTime.local());
        updateTime();

        const timer = setInterval(() => {
            updateTime();
            setShowColon((prev) => !prev);
        }, 1000);

        return () => clearInterval(timer);
    }, []);

    return (
        <span className={className}>
            {time.toFormat("HH")}
            <span className={`${showColon ? '' : 'invisible'}`}>:</span>
            {time.toFormat("mm")}
        </span>
    );
}

export default TClock;
