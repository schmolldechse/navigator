// place files you want to import through the `$lib` alias in this folder.
import { goto } from "$app/navigation";
import type {Stop} from "$models/station";

const gotoTimetable = (evaNumber: string, type: "departures" | "arrivals", startDate: string) => {
	goto(`/${evaNumber}/${type}?startDate=${encodeURIComponent(startDate)}`);
};

const writeStop = (stop?: Stop, fallbackName: string = ""): string => {
	if (!stop) return fallbackName;
	if (!stop.name) return fallbackName;
	if (!stop.nameParts || stop.nameParts.length === 0) return stop.name;
	return stop.nameParts.map(part => part.value).join("").trim();
}

export { gotoTimetable, writeStop };
