'use client';

import {useState} from "react";
import Image from "next/image";
import {useRouter} from "next/navigation";

type LocationResult = {
    id: number,
    name: string
}

export default function Home() {
    const router = useRouter();
    const [results, setResults] = useState<LocationResult[]>([]);

    const handleSearch = async (query: string) => {
        const response = await fetch(`https://hafas-v1.voldechse.wtf/locations/?query=${query}&addresses=false&results=5`, {method: 'GET'});
        if (!response.ok) return;

        const data = await response.json();

        const filtered: LocationResult[] = data
            .filter((location: any) => location.id && location.name)
            .map((location: any) => ({
                id: location.id,
                name: location.name
            }));
        setResults(filtered);
    }

    return (
        <div className="flex justify-center mt-[12.5%]">
            <div className="flex-col bg-[#000] p-4 rounded-xl w-[90%] max-w-[600px]">
                <div className="flex space-x-4">
                    <Image width={40} height={40} src={"search.svg"} alt={"Search icon"}/>
                    <input type="text" placeholder={"Search your station"}
                           onChange={(e) => {
                               const value: string = e.target.value;
                               if (value.trim().length === 0) {
                                   setResults([]);
                                   return;
                               }

                               handleSearch(value);
                           }}
                           className={"bg-[#000] text-xl p-3 w-full border rounded-lg border-slate-500 placeholder-slate-400 focus:outline-none"}
                    />
                </div>
                <div className={`flex-row ${results.length > 0 ? 'mt-4' : ''}`}>
                    {results.length > 0 ? (
                        results.map((location) => (
                            <p key={location.id}
                               className="text-white py-0.5 px-2 hover:bg-gray-700 hover:cursor-pointer rounded-md"
                               onClick={() => router.push(`/${location.id}/departures`)}
                            >
                                {location.name}
                            </p>
                        ))
                    ) : (<></>)}
                </div>
            </div>
        </div>
    );
}
