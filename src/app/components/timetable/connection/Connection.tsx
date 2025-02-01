import {useState} from "react";
import {Connection} from "@/app/lib/objects";
import {calculateDuration, writeName} from "@/app/lib/methods";
import CShowMore from "@/app/components/timetable/connection/CShowMore";
import { DateTime } from "luxon";
import CInfo from "@/app/components/timetable/connection/CInfo";

interface Props {
	connection: Connection;
	isDeparture: boolean;
    renderBorder: boolean;
    renderInfo: boolean;
}

const ConnectionComponent = ({connection, isDeparture, renderBorder, renderInfo}: Props) => {
    const [color, setColor] = useState<any>();

	const [expandVia, setExpandVia] = useState(false);
	const viaStops = connection?.viaStops
        ? (expandVia ? connection?.viaStops : connection?.viaStops?.slice(0, 3))
            .map(stop => writeName(stop))
            .join(" - ")
        : [];

    const isDelayed = () => {
        const diff = calculateDuration(
            DateTime.fromISO(isDeparture ? connection?.departure?.plannedTime : connection?.arrival?.plannedTime),
            DateTime.fromISO(isDeparture ? connection?.departure?.actualTime : connection?.arrival?.actualTime),
            "minutes"
        );
        return diff >= 1;
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

    const displayTime = (time: string) => DateTime.fromISO(time).setLocale("de-DE").toLocaleString(DateTime.TIME_24_SIMPLE);

    return (
		<div
			className={`${!connection?.cancelled ? '' : 'text-[#0a0a0a] bg-[#ededed]'}`}
		>
			{/* layout for smaller screens (under md) */}
			<div
				className={`p-2 md:hidden space-y-2 font-medium ${renderBorder ? 'border-t' : ''}`}
			>
                {/* first line */}
                <CInfo connection={connection} />

				{/* second line */}
				<span className={`${color ? 'py-[0.2rem] px-[0.8rem] rounded-xl' : ''} text-lg font-bold`}
					  style={{backgroundColor: color ? `${color.backgroundColor}` : ''}}
				>
                    {connection?.lineInformation?.fullName}{connection?.lineInformation?.additionalLineName ? ` / ${connection?.lineInformation?.additionalLineName}` : ''}
                </span>

				{/*
				third line
				renderInfo decides if the information will be shown. this is the case for single connections, or the last connection of a wing-train.
				 */}
                {renderInfo && (<div className="flex flex-row items-center font-semibold">
                    {/* scheduled time */}
                    <div className="flex-[1] flex flex-row items-center space-x-2 text-2xl">
                        <span>{displayTime(isDeparture ? connection?.departure?.plannedTime : connection?.arrival.plannedTime)}</span>
                        {isDelayed() && (
                            <span className={`bg-[#ededed] text-[#0a0a0a] text-sm px-[0.3rem] py-[0.05rem]`}>
                                {displayTime(isDeparture ? connection?.departure?.actualTime : connection?.arrival.actualTime)}
                            </span>
                        )}
                    </div>

                    {/* track */}
                    <span className="text-right text-2xl">{showPlatform()}</span>
				</div>)}

				{/* fourth line */}
				{viaStops && viaStops?.length > 0 && (<>
                    {/* additional line break is necessary here. renderInfo decides if time & platform will be shown. if not, viaStops will be right near of the lineInformation?.fullName */}
                    {!renderInfo && <br />}
                    <span className={"inline-block"}>
                        {viaStops}
                        {connection?.viaStops.length > 3 && (<CShowMore onClick={() => setExpandVia(!expandVia)}/>)}
                    </span>
                    <br />
				</>)}

				{/* fifth line */}
                <span className="text-2xl">
                    {isDeparture
                        ? writeName(connection?.actualDestination || connection?.destination, connection?.direction) // use actualDestination first, if available
                        : writeName(connection?.origin, connection?.provenance)}
                </span>
			</div>

            {/* layout for greater screens (above md) */}
            <span
                className={`w-full mx-auto hidden md:flex flex-col justify-between text-[30px] font-medium`}
            >
                {/* first row */}
                <div className={"flex flex-row"}>
                    <span className={`flex-[1] mr-8 ${renderBorder ? 'border-t' : ''}`}></span>
                    <span className={`flex-[4] mr-4 ${renderBorder ? 'border-t' : ''}`}>
                        <CInfo connection={connection} />
                    </span>
                    <span className={`flex-[1] ${renderBorder ? 'border-t' : ''}`}></span>
                </div>

                {/* second row */}
                <div className={"flex flex-row w-full"}>
                    {/* line nr */}
                    <span
                        className={`flex-[1] flex justify-end mr-8 text-xl ${!color ? 'font-semibold' : 'p-2 rounded-2xl px-4 font-bold'}`}
                    >
                        {connection?.lineInformation?.fullName}{connection?.lineInformation?.additionalLineName ? ` / ${connection?.lineInformation?.additionalLineName}` : ''}
                    </span>

                    {/* viaStops */}
                    <div className={"flex-[4] text-lg flex flex-row items-center space-x-2 mr-4"}>
                        {viaStops && viaStops.length > 0 && (<span className={"inline-block"}>
                            {viaStops}
                            {connection?.viaStops.length > 3 && (<CShowMore onClick={() => setExpandVia(!expandVia)}/>)}
                        </span>)}
                    </div>

                    {/* empty */}
                    <span className={"flex-[1]"}></span>
                </div>

                {/* third row */}
                <div className={"flex flex-row w-full"}>
                    {/* time information */}
                    <div className={"flex-[1] flex justify-end items-center space-x-2 mr-8"}>
                        {/* renderInfo decides if the information will be shown. this is the case for single connections, or the last connection of a wing-train. */}
                        {renderInfo && (<>
                            <span>{displayTime(isDeparture ? connection?.departure?.plannedTime : connection?.arrival.plannedTime)}</span>
						    {isDelayed() && (
							    <span
								    className={"text-[20px] px-[0.4rem] font-bold bg-[#ededed] text-[#0a0a0a]"}
                                >
                                    {displayTime(isDeparture ? connection?.departure?.actualTime : connection?.arrival.actualTime)}
                                </span>
						    )}
                        </>)}
                    </div>

                    {/* destination/ origin */}
                    <span className={"flex-[4] mr-4"}>
                        {isDeparture
                            ? writeName(connection?.actualDestination || connection?.destination, connection?.direction) // use actualDestination first, if available
                            : writeName(connection?.origin, connection?.provenance)}
                    </span>

                    {/* track */}
                    <span className={"flex-[1] flex text-right"}>{renderInfo && showPlatform()}</span>
                </div>
            </span>
		</div>
	);
}

export default ConnectionComponent;
