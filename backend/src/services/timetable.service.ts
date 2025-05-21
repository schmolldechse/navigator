import { t } from "elysia";
import { DateTimeObject } from "../types/datetime";
import { DateTime } from "luxon";
import { type Connection } from "navigator-core/src/models/connection";
import { mapConnection } from "../lib/mapping";

enum RequestType {
	DEPARTURES = "departures",
	ARRIVALS = "arrivals"
}

enum Profile {
	BAHNHOF = "bahnhof",
	RIS = "ris",
	HAFAS = "hafas"
}

class TimetableService {
	readonly query = t.Object({
		when: DateTimeObject({
			fieldName: "when",
			default: DateTime.now().startOf("minute").toISO({ suppressMilliseconds: true, suppressSeconds: true }),
			description: "Date to look for the timetable",
			error: "Parameter 'when' could not be parsed as a date."
		}),
		duration: t.Number({
			default: 60,
			minimum: 1,
			maximum: 720,
			description: "Duration in minutes to look for the timetable",
			error: "Parameter 'duration' must be a number."
		}),
		language: t.String({
			pattern: "^(de|en)$",
			default: "en",
			description: "Language of the timetable",
			error: "Parameter 'language' must be either 'de' or 'en'."
		})
	});

	retrieveBahnhofConnections = async (
		evaNumber: number,
		type: RequestType = RequestType.DEPARTURES,
		queryParams: typeof this.query.static
	): Promise<{ connections: Connection[] }[]> => {
		let apiUrl: URL = new URL(`https://bahnhof.de/api/boards/${type}?evaNumbers=${evaNumber}`);
		apiUrl.searchParams.append("duration", String(queryParams.duration));
		apiUrl.searchParams.append("locale", queryParams.language);

		const request = await fetch(apiUrl, { method: "GET" });
		if (!request.ok) return [];

		const response = await request.json();
		if (!response?.entries || !Array.isArray(response?.entries)) return [];

		return Object.values(response?.entries)
			.filter(Array.isArray)
			.map((journeyRaw) => ({
				connections: journeyRaw
					.filter((connectionRaw) => connectionRaw?.journeyID)
					.map((connectionRaw) => mapConnection(connectionRaw, type, true))
			}));
	};
}

export { TimetableService, RequestType, Profile };
