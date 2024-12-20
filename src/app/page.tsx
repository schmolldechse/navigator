'use client';

import {useState} from "react";
import Image from "next/image";

type LocationResult = {
    id: number,
    name: string
}

export default function Home() {
    const [results, setResults] = useState<LocationResult[]>([]);

    const handleSearch = async (query: string) => {
        const response = await fetch(`https://hafas.voldechse.wtf/locations/?query=${query}&addresses=false&results=5`, {method: 'GET'});
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
        <>
            <div className="backdrop-blur bg-transparent flex-col">
                <div className="flex">
                    <Image width={75} height={75} src={"search.svg"} alt={"Search icon"}/>
                    <input type="text" placeholder={"Search your station"}
                           onChange={(e) => {
                               const value: string = e.target.value;
                               if (value.trim().length === 0) {
                                   setResults([]);
                                   return;
                               }

                               handleSearch(value);
                           }}
                           className={ "bg-[#000]" }
                    />
                </div>
                <div className="flex-row">
                    {results.length > 0 ? (
                        results.map((location) => (
                            <p key={location.id} className="text-white">{location.name}</p>
                        ))
                    ) : (
                        <p>No results found</p>
                    )}
                </div>
            </div>
        </>
    );
}
