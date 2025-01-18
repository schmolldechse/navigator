import React, { useState, useRef } from "react";
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

const TimePicker = () => {
    const [currentDate, setCurrentDate] = useState(new Date());
    const [selectedDate, setSelectedDate] = useState(null);
    const [timeInput, setTimeInput] = useState({ hours: "00", minutes: "00" });
    const [showPicker, setShowPicker] = useState(false); // Toggle for calendar
    const pickerRef = useRef(null);
    const inputTimeout = useRef(null); // Ref for input timeout
    const buffer = useRef({ hours: "", minutes: "" }); // Buffer for input

    // Helper functions
    const prevMonth = () => setCurrentDate(subMonths(currentDate, 1));
    const nextMonth = () => setCurrentDate(addMonths(currentDate, 1));

    const renderCalendarDays = () => {
        const startDate = startOfWeek(startOfMonth(currentDate), { weekStartsOn: 1 });
        const endDate = endOfWeek(endOfMonth(currentDate), { weekStartsOn: 1 });

        let days = [];
        let day = startDate;

        while (day <= endDate) {
            days.push(day);
            day = addDays(day, 1);
        }

        return days.map((day) => (
            <div
                key={day}
                className={`p-2 text-center ${isSameMonth(day, currentDate)
                    ? ""
                    : "text-gray-400"} ${isSameDay(day, selectedDate)
                    ? "bg-red-500 text-white rounded-full"
                    : ""}`}
                onClick={() => setSelectedDate(day)}
            >
                {format(day, "d")}
            </div>
        ));
    };

    const adjustTimeByMinutes = (amount: number) => {
        setTimeInput((prev) => {
            let totalMinutes =
                parseInt(prev.hours) * 60 + parseInt(prev.minutes) + amount;

            if (totalMinutes < 0) {
                totalMinutes += 24 * 60;
            } else if (totalMinutes >= 24 * 60) {
                totalMinutes -= 24 * 60;
            }

            const newHours = Math.floor(totalMinutes / 60);
            const newMinutes = totalMinutes % 60;

            return {
                hours: String(newHours).padStart(2, "0"),
                minutes: String(newMinutes).padStart(2, "0"),
            };
        });
    };

    // Close the picker when clicking outside
    const handleClickOutside = (e) => {
        if (pickerRef.current && !pickerRef.current.contains(e.target)) {
            setShowPicker(false);
        }
    };

    React.useEffect(() => {
        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    const handleKeyPress = (e) => {
        const part = e.target.dataset.part;
        if (!part || !/^\d$/.test(e.key)) return;

        clearTimeout(inputTimeout.current);
        buffer.current[part] += e.key;

        if (buffer.current[part].length === 2) {
            // validate if two digits are entered
            const maxValue = part === "hours" ? 23 : 59;
            const validatedValue = Math.min(parseInt(buffer.current[part], 10), maxValue);

            setTimeInput((prev) => ({
                ...prev,
                [part]: String(validatedValue).padStart(2, "0"),
            }));

            // reset buffer
            buffer.current[part] = "";
        } else {
            // display single digit if one digit is entered
            setTimeInput((prev) => ({
                ...prev,
                [part]: `0${e.key}`,
            }));

            inputTimeout.current = setTimeout(() => buffer.current[part] = "", 500);
        }
    };

    return (
        <div className="relative rounded-xl text-black">
            {/* Input Field */}
            <div className="flex flex-row space-x-2 text-xl">
                <div className={"flex flex-row items-center space-x-4 p-2 grow nd-bg-lightgray rounded"}
                     onClick={() => setShowPicker(true)}
                >
                    <Image className={"p-1"} width={35} height={35} src={"./ui/calendar.svg"} alt={"CALENDAR"} />
                    <span
                        className={"w-full nd-bg-lightgray nd-fg-white cursor-pointer focus:outline-none"}
                    >
                        {format(selectedDate || currentDate, "dd.MM.yyyy")} - {timeInput.hours}:{timeInput.minutes}
                    </span>
                </div>
                <button className={"w-fit py-2 px-6 nd-bg-white rounded"}>Now</button>
            </div>

            {/* Calendar Picker */}
            {showPicker && (
                <div
                    ref={pickerRef}
                    className="absolute top-full mt-2 w-80 bg-white shadow-lg rounded p-4 z-10"
                >
                    {/* header */}
                    <div className="flex justify-between items-center mb-4">
                        <button onClick={prevMonth}>&lt; {format(subMonths(currentDate, 1), "MMM")}</button>
                        <div className={"font-bold"}>{format(currentDate, "MMMM yyyy")}</div>
                        <button onClick={nextMonth}>{format(addMonths(currentDate, 1), "MMM")} &gt;</button>
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
                    <div className="mt-4 flex items-center justify-between">
                        <button
                            className="bg-red-500 text-white px-3 py-1 rounded"
                            onClick={() => adjustTimeByMinutes(-15)}
                        >
                            -
                        </button>

                        <div className="flex items-center space-x-2">
                            <input
                                type="text"
                                name="hours"
                                data-part="hours"
                                value={timeInput.hours}
                                onKeyDown={handleKeyPress}
                                maxLength={2}
                                readOnly={true}
                                className="w-10 border rounded text-center"
                            />
                            :
                            <input
                                type="text"
                                name="minutes"
                                data-part="minutes"
                                value={timeInput.minutes}
                                onKeyDown={handleKeyPress}
                                maxLength={2}
                                readOnly={true}
                                className="w-10 border rounded text-center"
                            />
                        </div>

                        <button
                            className="bg-red-500 text-white px-3 py-1 rounded"
                            onClick={() => adjustTimeByMinutes(15)}
                        >
                            +
                        </button>
                    </div>

                    {/* select button */}
                    <button
                        className="mt-4 w-full bg-red-500 text-white py-2 rounded"
                        onClick={() => setShowPicker(false)}
                    >
                        Select
                    </button>
                </div>
            )}
        </div>
    );
};

export default TimePicker;
