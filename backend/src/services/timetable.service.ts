import { t } from "elysia";
import { DateTimeObject } from "../types/datetime";
import { DateTime } from "luxon";
import { mapConnection } from "../lib/mapping";
import { Connection } from "../models/core/models";

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

	retrieveRISConnections = async (
		evaNumber: number,
		type: RequestType = RequestType.DEPARTURES,
		queryParams: typeof this.query.static
	): Promise<{ connections: Connection[] }[]> => {
		const typeString = type === RequestType.DEPARTURES ? "departure" : "arrival";
		const transports = [
			"HIGH_SPEED_TRAIN",
			"INTERCITY_TRAIN",
			"INTER_REGIONAL_TRAIN",
			"REGIONAL_TRAIN",
			"CITY_TRAIN",
			"BUS",
			"FERRY",
			"SUBWAY",
			"TRAM",
			"SHUTTLE",
			"UNKNOWN"
		];

		let apiUrl: URL = new URL(
			`https://regio-guide.de/@prd/zupo-travel-information/api/public/ri/board/${typeString}/${evaNumber}`
		);
		apiUrl.searchParams.append("modeOfTransport", transports.join(","));
		apiUrl.searchParams.append("timeStart", queryParams.when.toUTC().toISO({ includeOffset: false, suppressMilliseconds: true })!);
		apiUrl.searchParams.append(
			"timeEnd",
			queryParams.when.plus({ minute: queryParams.duration }).toUTC().toISO({
				includeOffset: false,
				suppressMilliseconds: true
			})!
		);
		apiUrl.searchParams.append("expandTimeFrame", "TIME_END");
		apiUrl.searchParams.append("occupancy", String(true));

		const request = await fetch(apiUrl, { method: "GET" });
		if (!request.ok) return [];

		const response = await request.json();
		if (!response?.items || !Array.isArray(response?.items)) return [];

		return Object.values(response?.items)
			.map((journeyRaw) => ({ connections: [mapConnection(journeyRaw, type)] }));
	};

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
					.map((connectionRaw) => mapConnection(connectionRaw, type, { isBahnhofProfile: true }))
			}));
	};
}

export { TimetableService, RequestType, Profile };
