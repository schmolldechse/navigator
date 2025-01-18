import { Station } from "@/app/lib/objects";
import { useRouter } from "next/navigation";
import React, { useRef, useState } from "react";
import Image from "next/image";
import TimePicker from "@/app/components/ui/TimePicker";

const FTimetable: React.FC = ({ }) => {
    const router = useRouter();

    const [typeSelected, setTypeSelected] = useState<{ type: "departures" | "arrivals" }>({ type: 'departures' });
    const [stationSelected, setStationSelected] = useState<{ selected: boolean | false, station?: Station | undefined }>({
        selected: false,
        station: undefined,
    });

    // queries from /api/v1/station
    const [query, setQuery] = useState<Station[]>([]);
    const [focusedQuery, setFocusedQuery] = useState<number | undefined>(null);

    // input field to search stations
    const [inputValue, setInputValue] = useState<string>("");

    // timeout, so that a request is only sent after 0.5s if nothing has been entered
    const timeoutRef = useRef<NodeJS.Timeout | null>(null);

    const handleInput = (value: string) => {
        setInputValue(value);
        if (value.trim().length === 0) {
            setQuery([]);
            return;
        }

        if (timeoutRef.current) clearTimeout(timeoutRef.current);
        timeoutRef.current = setTimeout(() => searchStations(value), 500);
    }

    const searchStations = async (query: string) => {
        const response = await fetch(`/api/v1/station`, {
            method: 'POST',
            body: JSON.stringify({ query: query })
        });
        if (!response.ok) return;

        const data = await response.json();
        if (!data.entries || data.entries.length === 0) return;

        const filtered: Station[] = data.entries
            .filter((location: any) => location.evaNr && location.name)
            .map((location: any) => location as Station);
        setQuery(filtered);
        setFocusedQuery(null);
    }

    const handleKeyInput = (e: React.KeyboardEvent<HTMLDivElement>) => {
        if (query.length === 0) return;

        switch (e.key) {
            case 'ArrowDown':
            case 'Tab':
                e.preventDefault();
                setFocusedQuery((prev) => (prev === null || prev === query.length - 1 ? 0 : prev + 1));
                break;
            case 'ArrowUp':
                e.preventDefault();
                setFocusedQuery((prev) => (prev === null || prev === 0 ? query.length - 1 : prev - 1));
                break;
            case 'Enter':
                if (focusedQuery === null) return;
                selectStation(query[focusedQuery]);
                break;
            default:
                break;
        }
    }

    const selectStation = (station: Station) => {
        setStationSelected({ selected: true, station: station });
        setInputValue(station.name);
    }

    return (
        <div className={"px-10 py-7 w-[40%] nd-bg-darkgray flex flex-col space-y-4 nd-fg-white"}>
            <div className={"ml-auto items-center space-x-4"}>
                <span className={`cursor-pointer ${typeSelected.type === 'departures' ? 'nd-fg-aqua' : 'nd-fg-white'}`} onClick={() => setTypeSelected({ type: 'departures' })}>Departures</span>
                <span className={"text-xl nd-fg-white"}>|</span>
                <span className={`cursor-pointer ${typeSelected.type === 'arrivals' ? 'nd-fg-aqua' : 'nd-fg-white'}`} onClick={() => setTypeSelected({ type: 'arrivals' })}>Arrivals</span>
            </div>

            <div className={"relative flex flex-col space-x-2 nd-bg-lightgray p-2 rounded-xl"} onKeyDown={handleKeyInput}>
                <div className={"flex flex-row"}>
                    <Image width={35} height={35} src={"/search.svg"} alt={"SEARCH ICON"} />
                    <input type="text" placeholder={"Search a station"}
                           onChange={(e) => handleInput(e.target.value)}
                           onSelect={() => setStationSelected((prev) => ({ ...prev, selected: false }))}
                           className={"nd-bg-lightgray text-2xl p-1 w-full focus:outline-none"}
                           value={inputValue}
                    />
                </div>
                <div className={`${!stationSelected.selected && query.length > 0 ? "absolute top-full left-0 z-50 flex flex-col mt-2 nd-bg-lightgray rounded" : "hidden"}`}>
                    {!stationSelected.selected && query.length > 0 && query.map((location: Station, index) => (
                        <p
                            key={location.evaNr}
                            className={`m-1 text-white py-0.5 px-2 cursor-pointer nd-bg-lightgray hover:bg-gray-700 rounded`}
                            onClick={() => selectStation(location)}
                            tabIndex={0}
                            role="button"
                            aria-pressed={focusedQuery === index}
                        >
                            {location.name}
                        </p>
                    ))}
                </div>
            </div>

            <div className={"flex flex-col"}>
                <span>Pick a time:</span>
                <TimePicker />
            </div>

            <button
                className={"w-full nd-bg-aqua p-2 rounded nd-fg-black font-medium text-xl"}
                onClick={() => {
                    if (!stationSelected.selected) return;
                    if (!stationSelected.station) return;
                    router.push(`/${stationSelected.station?.evaNr}/${typeSelected.type}`);
                }}
            >
                Request
            </button>
        </div>
    );
}

export default FTimetable;
