"use client";

import React, {useState, useRef, useEffect} from "react";
import {
    addDays,
    addMonths,
    endOfMonth,
    endOfWeek,
    format,
    isSameDay,
    isSameMonth,
    startOfMonth,
    startOfWeek,
    subMonths,
} from "date-fns";
import Image from "next/image";

interface Props {
    onChangedDate: (date: Date) => void;
}

const TimePicker: React.FC<Props> = ({ onChangedDate }) => {
    const [selectedDate, setSelectedDate] = useState<Date>(new Date());

    const [showPicker, setShowPicker] = useState(false); // Toggle for calendar
    const pickerRef = useRef(null);

    const inputTimeout = useRef(null); // Ref for input timeout
    const buffer = useRef({ hours: "", minutes: "" }); // Buffer for input

    // Helper functions
    const prevMonth = () => setSelectedDate(subMonths(selectedDate, 1));
    const nextMonth = () => setSelectedDate(addMonths(selectedDate, 1));

    const renderCalendarDays = () => {
        const startDate = startOfWeek(startOfMonth(selectedDate), { weekStartsOn: 1 });
        const endDate = endOfWeek(endOfMonth(selectedDate), { weekStartsOn: 1 });

        let days = [];
        let day = startDate;

        while (day <= endDate) {
            days.push(day);
            day = addDays(day, 1);
        }

        return days.map((day) => (
            <div
                key={day}
                className={`p-2 text-center ${isSameMonth(day, selectedDate)
                    ? ""
                    : "text-gray-400"} ${isSameDay(day, selectedDate)
                    ? "nd-bg-aqua nd-fg-black font-bold rounded-full"
                    : ""}`}
                onClick={() => setSelectedDate((prev) => {
                    const updatedDate = new Date(day);
                    updatedDate.setHours(prev.getHours());
                    updatedDate.setMinutes(prev.getMinutes());
                    return updatedDate;
                })}
            >
                {format(day, "d")}
            </div>
        ));
    };

    const adjustTimeByMinutes = (amount: number) => {
        setSelectedDate((prev) => {
            const newDate = new Date(prev);
            newDate.setMinutes(newDate.getMinutes() + amount);
            return newDate;
        });
    };

    useEffect(() => {
        const handleClickOutside = (e: MouseEvent) => {
            if (pickerRef.current && !pickerRef.current.contains(e.target)) {
                setShowPicker(false);
            }
        }

        if (selectedDate) onChangedDate(selectedDate);

        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, [selectedDate]);

    const handleKeyPress = (e) => {
        const part = e.target.dataset.part;
        if (!part || (part !== "minutes" || part !== "hours"))
        if (!/^\d+$/.test(e.key)) return;

        clearTimeout(inputTimeout.current);
        buffer.current[part] += e.key;

        if (buffer.current[part].length === 2) {
            // validate if two digits are entered
            const maxValue = part === "hours" ? 23 : 59;
            const validatedValue = Math.min(parseInt(buffer.current[part], 10), maxValue);

            setSelectedDate((prev) => {
                const newDate: Date = new Date(prev);
                if (part === "hours") newDate.setHours(validatedValue);
                if (part === "minutes") newDate.setMinutes(validatedValue);
                return newDate;
            });

            // reset buffer
            buffer.current[part] = "";
        } else {
            setSelectedDate((prev) => {
                const newDate: Date = new Date(prev);
                if (part === "hours") newDate.setHours(parseInt(e.key, 10) || 0);
                if (part === "minutes") newDate.setMinutes(parseInt(e.key, 10) || 0);
                return newDate;
            });

            inputTimeout.current = setTimeout(() => buffer.current[part] = "", 500);
        }
    };

    return (
        <div className="relative rounded-xl text-black">
            {/* Input Field */}
            <div className="flex flex-row space-x-2">
                <div className={"flex flex-row items-center space-x-2 md:space-x-4 p-2 grow nd-bg-lightgray rounded"}
                     onClick={() => setShowPicker(true)}
                >
                    <Image className={"p-1 md:h-[35px] md:w-[35px]"} width={25} height={25} src={"/ui/calendar.svg"} alt={"CALENDAR"} />
                    <span
                        className={"w-full text-base md:text-2xl nd-bg-lightgray nd-fg-white cursor-pointer focus:outline-none"}
                    >
                        {format(selectedDate, "dd.MM.yyyy - HH:mm")}
                    </span>
                </div>
                <button className={"w-fit py-2 px-3 md:px-6 nd-bg-white rounded font-bold"} onClick={() => setSelectedDate(new Date())}>Now</button>
            </div>

            {/* Calendar Picker */}
            {showPicker && (
                <div
                    ref={pickerRef}
                    className="absolute top-1/2 left-1/2 mt-8 transform -translate-x-1/2 p-4 w-80 md:top-full md:left-auto md:transform-none md:mt-2 md:w-auto z-10 shadow-lg rounded nd-bg-lightgray nd-fg-white"
                >
                    {/* header */}
                    <div className="flex justify-between items-center mb-4">
                        <button onClick={prevMonth}>&lt; {format(subMonths(selectedDate, 1), "MMM")}</button>
                        <div className={"font-bold"}>{format(selectedDate, "MMMM yyyy")}</div>
                        <button onClick={nextMonth}>{format(addMonths(selectedDate, 1), "MMM")} &gt;</button>
                    </div>

                    {/* calendar grid */}
                    <div className="grid grid-cols-7 gap-1 text-sm">
                        {["Mo", "Tu", "We", "Th", "Fr", "Sa", "Su"].map((day) => (
                            <div key={day} className="font-bold text-center">
                                {day}
                            </div>
                        ))}
                        {renderCalendarDays()}
                    </div>

                    {/* time inputs */}
                    <div className="mt-4 flex items-center justify-between font-bold space-x-2">
                        <button
                            className="nd-bg-aqua text-black px-4 md:px-3 py-1 rounded"
                            onClick={() => adjustTimeByMinutes(-15)}
                        >
                            -
                        </button>

                        <div className="flex items-center space-x-2">
                            <input
                                type="text"
                                name="hours"
                                data-part="hours"
                                value={String(selectedDate.getHours()).padStart(2, "0")}
                                onKeyDown={handleKeyPress}
                                maxLength={2}
                                readOnly={true}
                                className="w-full border rounded text-center text-xl nd-bg-lightgray nd-fg-white border-none focus:outline-none"
                            />
                            <span>:</span>
                            <input
                                type="text"
                                name="minutes"
                                data-part="minutes"
                                value={String(selectedDate.getMinutes()).padStart(2, "0")}
                                onKeyDown={handleKeyPress}
                                maxLength={2}
                                readOnly={true}
                                className="w-full border rounded text-center text-xl nd-bg-lightgray nd-fg-white border-none focus:outline-none"
                            />
                        </div>

                        <button
                            className="nd-bg-aqua text-black px-4 md:px-3 py-1 rounded"
                            onClick={() => adjustTimeByMinutes(15)}
                        >
                            +
                        </button>
                    </div>

                    {/* select button */}
                    <button
                        className="mt-4 w-full nd-bg-aqua font-bold nd-fg-black py-2 rounded"
                        onClick={() => {
                            setShowPicker(false);
                            onChangedDate(selectedDate);
                        }}
                    >
                        Select
                    </button>
                </div>
            )}
        </div>
    );
};

export default TimePicker;
