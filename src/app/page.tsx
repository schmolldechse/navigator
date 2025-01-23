'use client';

import { useState } from "react";
import Logo from "./components/branding/logo";
import FNavbar from "./components/frontpage/FNavbar";
import FTimetable from "./components/frontpage/FTimetable";

export default function Home() {
    const [selected, setSelected] = useState<"timetable" | "route_planner">('timetable');

    return (
        <div className={"m-4"}>
            <div className={"flex justify-between items-center pr-4"}>
                <Logo />
                <FNavbar type={selected} setType={setSelected} />
            </div>

            <div className={"flex items-center justify-center"}>
                {selected === 'timetable' ? (
                    <FTimetable />
                ) : (
                    <></>
                )}
            </div>
        </div>
    );
}
