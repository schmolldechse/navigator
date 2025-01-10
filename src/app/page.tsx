'use client';

import {useRef, useState} from "react";
import Image from "next/image";
import {useRouter} from "next/navigation";
import {Station} from "@/app/lib/objects";

export default function Home() {
    const router = useRouter();
    const [stations, setStations] = useState<Station[]>([]);

    const [focusedIndex, setFocusedIndex] = useState<number | undefined>(null);

    const timeoutRef = useRef<NodeJS.Timeout | null>(null);

    const handleInput = (value: string) => {
        if (value.trim().length === 0) {
            setStations([]);
            return;
        }

        if (timeoutRef.current) clearTimeout(timeoutRef.current);
        timeoutRef.current = setTimeout(() => searchStations(value), 500);
    }

    const searchStations = async (query: string) => {
        const response = await fetch(`/api/v1/station`, {
            method: 'POST',
            body: JSON.stringify({query: query})
        });
        if (!response.ok) return;

        const data = await response.json();
        if (!data.entries || data.entries.length === 0) return;

        const filtered: Station[] = data.entries
            .filter((location: any) => location.evaNr && location.name)
            .map((location: any) => location as Station);
        setStations(filtered);
        setFocusedIndex(null);
    }

    const handleKeyInput = (e: React.KeyboardEvent<HTMLDivElement>) => {
        if (stations.length === 0) return;

        switch (e.key) {
            case 'ArrowDown':
            case 'Tab':
                e.preventDefault();
                setFocusedIndex((prev) => (prev === null || prev === stations.length - 1 ? 0 : prev + 1));
                break;
            case 'ArrowUp':
                e.preventDefault();
                setFocusedIndex((prev) => (prev === null || prev === 0 ? stations.length - 1 : prev - 1));
                break;
            case 'Enter':
                if (focusedIndex === null) return;
                router.push(`/${stations[focusedIndex].evaNr}/departures`);
                break;
            default:
                break;
        }
    }

    return (
        <div className="flex justify-center mt-[12.5%]" onKeyDown={handleKeyInput}>
            <div className="flex-col bg-[#000] p-4 rounded-xl w-[90%] max-w-[600px]">
                <div className="flex space-x-4">
                    <Image width={40} height={40} src={"/search.svg"} alt={"Search icon"}/>
                    <input type="text" placeholder={"Search your station"}
                           onChange={(e) => handleInput(e.target.value)}
                           className={"bg-[#000] text-xl p-3 w-full border rounded-lg border-slate-500 placeholder-slate-400 focus:outline-none"}
                    />
                </div>

                <div className={`flex-row ${stations.length > 0 ? 'mt-4' : ''}`}>
                    {stations.length > 0 && stations.map((location: Station, index) => (
                        <p
                            key={location.evaNr}
                            className={`text-white py-0.5 px-2 hover:bg-gray-700 hover:cursor-pointer rounded-md ${focusedIndex === index ? 'bg-gray-700' : ''}`}
                            onClick={() => router.push(`/${location.evaNr}/departures`)}
                            tabIndex={0}
                            role="button"
                            aria-pressed={focusedIndex === index}
                        >
                            {location.name}
                        </p>
                    ))}
                </div>
            </div>
        </div>
    );
}
