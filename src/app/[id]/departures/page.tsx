'use client';

import {useParams} from "next/navigation";

export default function Departures() {
    const params = useParams();
    const id = params.id;

    console.log(id);

    return (
        <>
            <p className={"text-white"}>Hallo!</p>
        </>
    )
}
