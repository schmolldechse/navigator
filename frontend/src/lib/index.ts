// place files you want to import through the `$lib` alias in this folder.
import type { Stop } from "$models/station";

const writeStop = (stop?: Stop, fallbackName: string = ""): string => {
	if (!stop) return fallbackName;
	if (!stop.name) return fallbackName;
	if (!stop.nameParts || stop.nameParts.length === 0) return stop.name;
	return stop.nameParts
		.map((part) => part.value)
		.join("")
		.trim();
};

const mapStops = (entry: any): Stop[] | null => {
	if (!entry) return null;
	if (!Array.isArray(entry)) entry = [entry];
	return entry.map((rawStop: any) => ({
		evaNumber: rawStop?.evaNumber ?? rawStop?.id,
		name: rawStop?.name,
		cancelled: rawStop?.canceled ?? rawStop?.cancelled ?? false,
		additional: rawStop?.additional ?? undefined,
		separation: rawStop?.separation ?? undefined,
		nameParts:
			rawStop?.nameParts?.map((rawPart: any) => ({
				type: rawPart?.type,
				value: rawPart?.value
			})) ?? undefined
	}));
};

export { writeStop, mapStops };
