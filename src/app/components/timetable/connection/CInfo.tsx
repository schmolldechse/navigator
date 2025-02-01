import {Connection, Message, Stop} from "@/app/lib/objects";
import React from "react";
import ITrackChanged from "@/app/components/timetable/connection/icons/ITrackChanged";
import ICanceledStops from "@/app/components/timetable/connection/icons/ICanceledStops";
import IAdditionalStops from "@/app/components/timetable/connection/icons/IAdditionalStops";
import IBicycleReservationRequired from "@/app/components/timetable/connection/icons/IBicycleReservationRequired";
import IBicycleTransport from "@/app/components/timetable/connection/icons/IBicycleTransport";
import IAccessibilityWarning from "@/app/components/timetable/connection/icons/IAccessibilityWarning";
import IUnplannedInfo from "@/app/components/timetable/connection/icons/IUnplannedInfo";
import ICancelledTrip from "@/app/components/timetable/connection/icons/ICancelledTrip";
import ITicketInformation from "@/app/components/timetable/connection/icons/ITicketInformation";
import IAdditionalCoaches from "@/app/components/timetable/connection/icons/IAdditionalCoaches";
import IMissingCoaches from "@/app/components/timetable/connection/icons/IMissingCoaches";
import INoFood from "@/app/components/timetable/connection/icons/INoFood";
import {mapStops, writeName} from "@/app/lib/methods";

interface Props {
    connection: Connection;
}

interface ValidMessage {
    type: string;
    iconComponent?: React.FC<{ width?: number, height?: number }>;
}

const CInfo = ({ connection }: Props) => {
    const validMessages: ValidMessage[] = [
        { type: "bicycle-transport", iconComponent: IBicycleTransport },
        { type: "bicycle-reservation-required", iconComponent: IBicycleReservationRequired },
        { type: "canceled-stops", iconComponent: ICanceledStops },
        { type: "additional-stops", iconComponent: IAdditionalStops },
        { type: "track-changed", iconComponent: ITrackChanged },
        { type: "accessibility-warning", iconComponent: IAccessibilityWarning }, // e.g. without "Vehicle-mounted boarding aid", "No disabled WC on the train",
        { type: "unplanned-info", iconComponent: IUnplannedInfo },
        { type: "canceled-trip", iconComponent: ICancelledTrip },
        { type: "ticket-information", iconComponent: ITicketInformation }, // if a train has a additionalLineName
        { type: "additional-coaches", iconComponent: IAdditionalCoaches },
        { type: "missing-coaches", iconComponent: IMissingCoaches },
        { type: "replacement-service" },
        { type: "no-food", iconComponent: INoFood },
        { type: "no-first-class" },
        { type: "reservation-required" },
        { type: "general-warning" },
        { type: "chanced-sequence" },
        { type: "lift-warning" },
        { type: "no-wi-fi" },
        { type: "bicycle-warning" },
        { type: "no-onward-journey" }, // X as SVG, like in canceled-trip
        { type: "continuation-by" }
    ];
    if (!connection?.messages) return null;

    const collectMessages = (messages?: Record<string, Message[] | []>): Message[] => {
        if (!messages) return [];
        return Object.values(messages).flat();
    }

    const filteredMessages = collectMessages(connection?.messages).filter((message) => {
        const isValidMessage = validMessages.some((validMessage) => validMessage.type === message.type);

        if (connection?.cancelled) {
            return isValidMessage && message.important;
        }

        return isValidMessage;
    });
    if (filteredMessages.length === 0) return null;

    const formatMessage = (message: Message): string => {
        if (!message?.type || !message?.text) return "Invalid message object";
        if (!message?.links || message?.links.length === 0) return message?.text;

        let formatted = message?.text;
        message?.links?.forEach((link, index) => {
            const placeholder = `{{${index}}}`;

            if (link?.type === "station") {
                const stop: Stop = mapStops(link)[0];
                formatted = formatted.replace(placeholder, writeName(stop, link?.name));
            } else if (link?.type === "line") formatted = formatted.replace(placeholder, link?.lineName);
            else if (link?.type === "link") formatted = formatted.replace(placeholder, link?.label);
            else formatted = formatted.replace(placeholder, link?.name);
        });
        return formatted;
    }

    return (<>
        {filteredMessages.map((message: Message, index: number) => {
            const validMessage = validMessages.find((validMessage: ValidMessage) => validMessage.type === message.type);
            return (
                <div key={index} className={`flex flex-row space-x-2 my-1 py-0.5 text-xl ${message?.change ? 'nd-bg-white nd-fg-black' : ''}`}>
                    {validMessage?.iconComponent && <validMessage.iconComponent height={25} width={25} />}
                    <span>{formatMessage(message)}</span>
                </div>
            )
        })}
    </>)
};

export default CInfo;
