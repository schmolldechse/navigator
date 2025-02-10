// place files you want to import through the `$lib` alias in this folder.
import { goto } from "$app/navigation";

const gotoTimetable = (evaNumber: string, type: "departures" | "arrivals", startDate: string) => {
	goto(`/${evaNumber}/${type}?startDate=${encodeURIComponent(startDate)}`);
};

export { gotoTimetable };
