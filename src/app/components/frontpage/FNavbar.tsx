'use client';

interface Props {
    type: "timetable" | "route_planner";
    setType: (type: "timetable" | "route_planner") => void;
}

const FNavbar = ({ type, setType }: Props) => {
    return (
        <span className={"montserrat-regular text-base md:text-xl flex items-center space-x-4"}>
            <span
                onClick={() => setType('timetable')}
                className={`cursor-pointer ${type === 'timetable' ? 'border-b-2 border-white' : ''}`}
            >
                Timetable
            </span>
            <span
                onClick={() => setType('route_planner')}
                className={`cursor-pointer ${type === 'route_planner' ? 'border-b-2 border-white' : ''}`}
            >
                Route Planner
            </span>
        </span>
    );
}

export default FNavbar;
