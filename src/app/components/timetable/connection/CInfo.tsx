import {Connection, Message} from "@/app/lib/objects";
import React from "react";
import ITrackChanged from "@/app/components/timetable/connection/icons/ITrackChanged";
import ICanceledStops from "@/app/components/timetable/connection/icons/ICanceledStops";
import IAdditionalStops from "@/app/components/timetable/connection/icons/IAdditionalStops";
import IBicycleReservationRequired from "@/app/components/timetable/connection/icons/IBicycleReservationRequired";
import IBicycleTransport from "@/app/components/timetable/connection/icons/IBicycleTransport";
import IAccessibilityWarning from "@/app/components/timetable/connection/icons/IAccessibilityWarning";

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
        { type: "unplanned-info" },
        { type: "canceled-trip" },
        { type: "ticket-information" }, // if a train has a additionalLineName
        { type: "additional-coaches" },
        { type: "missing-coaches" },
        { type: "replacement-service" },
        { type: "no-food" },
        { type: "no-first-class" },
        { type: "reservation-required" },
        { type: "general-warning" },
        { type: "chanced-sequence" },
        { type: "lift-warning" }
    ]
    if (!connection?.messages) return null;

    const collectMessages = (messages?: Record<string, Message[] | []>): Message[] => {
        if (!messages) return [];
        return Object.values(messages).flat();
    }

    const filteredMessages = collectMessages(connection?.messages).filter((message) =>
        validMessages.some((validMessage) => validMessage.type === message.type)
    );
    if (filteredMessages.length === 0) return null;

    return (<>
        {filteredMessages.map((message: Message, index: number) => {
            const validMessage = validMessages.find((validMessage: ValidMessage) => validMessage.type === message.type);
            return (
                <div key={index} className={`flex flex-row space-x-2 my-1 py-0.5 text-xl ${message?.change ? 'nd-bg-white nd-fg-black' : ''}`}>
                    {validMessage?.iconComponent && <validMessage.iconComponent height={25} width={25} />}
                    <span>{message.text}</span>
                </div>
            )
        })}
    </>)
};

export default CInfo;
