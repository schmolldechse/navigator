import { Station } from "@/app/lib/objects";
import { useRouter } from "next/navigation";
import React, { useState } from "react";
import TimePicker from "@/app/components/ui/TimePicker";
import StationSearch from "@/app/components/ui/StationSearch";

const FTimetable: React.FC = ({ }) => {
    const router = useRouter();

    const [typeSelected, setTypeSelected] = useState<{ type: "departures" | "arrivals" }>({ type: 'departures' });
    const [stationSelected, setStationSelected] = useState<Station | undefined>(undefined);

    return (
        <div className={"px-10 py-7 w-[40%] nd-bg-darkgray flex flex-col space-y-4 nd-fg-white"}>
            <div className={"ml-auto items-center space-x-4"}>
                <span className={`cursor-pointer ${typeSelected.type === 'departures' ? 'nd-fg-aqua' : 'nd-fg-white'}`} onClick={() => setTypeSelected({ type: 'departures' })}>Departures</span>
                <span className={"text-xl nd-fg-white"}>|</span>
                <span className={`cursor-pointer ${typeSelected.type === 'arrivals' ? 'nd-fg-aqua' : 'nd-fg-white'}`} onClick={() => setTypeSelected({ type: 'arrivals' })}>Arrivals</span>
            </div>

            <StationSearch  onSelectStation={(station) => setStationSelected(station)} />

            <div className={"flex flex-col"}>
                <span>Pick a time:</span>
                <TimePicker />
            </div>

            <button
                className={"w-full nd-bg-aqua p-2 rounded nd-fg-black font-medium text-xl"}
                onClick={() => {
                    if (!stationSelected) return;
                    router.push(`/${stationSelected?.evaNr}/${typeSelected.type}`);
                }}
            >
                Request
            </button>
        </div>
    );
}

export default FTimetable;
