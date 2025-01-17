interface Props {
    type: "timetable" | "route_planner";
}

const FNavbar: React.FC<Props> = ({ type }) => {
    return (
        <span className={"montserrat-regular flex items-center space-x-4"}>
            <span className={`cursor-pointer ${type === 'timetable' ? 'border-b-2 border-white' : ''}`}>Timetable</span>
            <span className={`cursor-pointer ${type === 'route_planner' ? 'border-b-2 border-white' : ''}`}>Route Planner</span>
        </span>
    );
}

export default FNavbar;
