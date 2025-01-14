import {useState} from "react";
import {Connection} from "@/app/lib/objects";
import {writeName} from "@/app/lib/methods";
import ShowMore from "@/app/components/show-more";

interface ScheduledProps {
	connection: Connection,
	isDeparture: boolean,
}

const ScheduledComponent: React.FC<ScheduledProps> = ({connection, isDeparture}) => {
    const [color, setColor] = useState<any>();

	const [expandVia, setExpandVia] = useState(false);
	const displayedViaStops = expandVia ? connection?.viaStops : connection?.viaStops.slice(0, 3);
	const viaStops = displayedViaStops.map(writeName).join(" – ");

    const isDelayed = () => {
        const planned = new Date(isDeparture ? connection.departure.plannedTime : connection.arrival.plannedTime);
        const actual = new Date(isDeparture ? connection.departure.actualTime : connection.arrival.actualTime);

        const difference = (actual.getTime() - planned.getTime()) / 60000; // in min
        return difference >= 1;
    }

    const showPlatform = () => {
        const platform = isDeparture ? connection?.departure : connection.arrival;
        const isSame = platform.plannedPlatform === platform.actualPlatform;

        return (
            <span className={`md:w-full ${isSame ? "" : "bg-[#ededed] text-black px-2 md:px-0"}`}>
                {isSame ? platform.plannedPlatform : platform.actualPlatform}
            </span>
		);
	}

    const displayTime = (time: string) => new Date(time).toLocaleTimeString("de-DE", {hour: "2-digit", minute: "2-digit"});

    return (
		<div
			className={`${!connection?.cancelled ? '' : 'text-[#0a0a0a] bg-[#ededed]'}`}
		>
			{/* layout for smaller screens (under md) */}
			<div
				className={`p-2 md:hidden border-t space-y-2 font-medium`}
			>
				{/* first line */}
				<span className={`${color ? 'py-[0.2rem] px-[0.8rem] rounded-xl' : ''} text-lg font-bold`}
					  style={{backgroundColor: color ? `${color.backgroundColor}` : ''}}
				>
                    {connection.lineInformation?.fullName}
                </span>

				{/* second line */}
				<div className="flex flex-row items-center font-semibold">
					{/* scheduled time */}
					<div className="flex-[1] flex flex-row items-center space-x-2 text-2xl">
                        <span>{displayTime(isDeparture ? connection?.departure?.plannedTime : connection?.arrival.plannedTime)}</span>
						{isDelayed() && (
							<span
								className={`bg-[#ededed] text-[#0a0a0a] text-sm px-[0.3rem] py-[0.05rem]`}
							>
                                {displayTime(isDeparture ? connection?.departure?.actualTime : connection?.arrival.actualTime)}
                            </span>
						)}
					</div>

					{/* track */}
					<span className="text-right text-2xl">{showPlatform()}</span>
				</div>

				{!connection?.cancelled && connection?.viaStops?.length > 0 && (<>
					{/* third line */}
                    <marquee className={"text-base"}>{connection?.viaStops?.map(writeName).join(" – ")}</marquee>
                    <br/>
				</>)}

				{/* fourth line */}
				<span className="text-2xl">{writeName(isDeparture ? connection?.destination : connection?.origin)}</span>
			</div>

            {/* layout for greater screens (above md) */}
            <span
                className={`w-full mx-auto hidden md:flex flex-col justify-between text-[30px] font-medium ${connection?.cancelled ? 'bg-[#ededed] text-black' : ''}`}
            >
                {/* first row */}
                <div className={"flex flex-row"}>
                    <span className={"flex-[1] mr-8 border-t"}></span>
                    <span className={"flex-[4] mr-4 border-t"}></span>
                    <span className={"flex-[1] border-t"}></span>
                </div>

                {/* second row */}
                <div className={"flex flex-row w-full"}>
                    {/* line nr */}
                    <span
                        className={`flex-[1] flex justify-end mr-8 text-xl ${!color ? 'font-semibold' : 'p-2 rounded-2xl px-4 font-bold'}`}
                    >
                        {connection?.lineInformation!!.fullName}
                    </span>

                    {/* viaStops */}
                    <div className={"flex-[4] text-lg flex flex-row items-center space-x-2 mr-4"}>
                        {viaStops.length > 0 && (<span className={"inline-block"}>
                            {viaStops}
                            {connection?.viaStops.length > 3 && (<ShowMore onClick={() => setExpandVia(!expandVia)}/>)}
                        </span>)}
                    </div>

                    {/* empty */}
                    <span className={"flex-[1]"}></span>
                </div>

                {/* third row */}
                <div className={"flex flex-row w-full"}>
                    {/* time information */}
                    <div className={"flex-[1] flex justify-end items-center space-x-2 mr-8"}>
                        <span>{displayTime(isDeparture ? connection?.departure?.plannedTime : connection?.arrival.plannedTime)}</span>
						{isDelayed() && (
							<span
								className={"text-[20px] px-[0.4rem] font-bold bg-[#ededed] text-[#0a0a0a]"}
                            >
                                {displayTime(isDeparture ? connection?.departure?.actualTime : connection?.arrival.actualTime)}
                            </span>
						)}
                    </div>

                    {/* destination/ origin */}
                    <span
                        className={"flex-[4] mr-4"}
                    >
                        {writeName(isDeparture ? connection?.destination : connection?.origin)}
                    </span>

                    {/* track */}
                    <span className={"flex-[1] text-right"}>{showPlatform()}</span>
                </div>
            </span>
		</div>
	);
}

export default ScheduledComponent;
