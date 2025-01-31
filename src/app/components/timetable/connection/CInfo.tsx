import {Connection, Message} from "@/app/lib/objects";
import Image from "next/image";

interface Props {
    connection: Connection;
}

interface ValidMessage {
    type: string;
    svgSource?: string,
}

const CInfo = ({ connection }: Props) => {
    const validMessages: ValidMessage[] = [
        { type: "bicycle-transport", svgSource: "/timetable/infos/bicycle-transport.svg" },
        { type: "bicycle-reservation-required" },
        { type: "canceled-stops" },
        { type: "additional-stops" },
        { type: "track-changed" },
        { type: "accessibility-warning" }, // e.g. without "Vehicle-mounted boarding aid", "No disabled WC on the train",
        { type: "unplanned-info" },
        { type: "canceled-trip" },
        { type: "ticket-information" }, // if a train has a additionalLineName
        { type: "additional-coaches" }
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
                <div key={index} className={"flex flex-row"}>
                    {message.text}
                </div>
            )
        })}
    </>)
};

export default CInfo;
