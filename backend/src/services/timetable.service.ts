import { t } from "elysia";
import { DateTimeObject } from "../types/datetime";
import { DateTime } from "luxon";

enum RequestType {
	DEPARTURES = "departures",
	ARRIVALS = "arrivals"
}

enum Profile {
	BAHNHOF,
	RIS,
	HAFAS
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
		results: t.Number({
			default: 1000,
			minimum: 1,
			maximum: 1000,
			description: "Maximum number of results to return",
			error: "Parameter 'maxResults' must be a number."
		}),
		language: t.String({
			pattern: "^(de|en)$",
			default: "en",
			description: "Language of the timetable",
			error: "Parameter 'language' must be either 'de' or 'en'."
		})
	});

	retrieveConnections = async (
		evaNumber: number,
		type: RequestType = RequestType.DEPARTURES,
		profile: Profile = Profile.RIS,
		queryParams: typeof this.query.static
	): Promise<Connection[]> => {
		let apiUrl: URL;
		switch (profile) {
			case Profile.BAHNHOF:
				apiUrl = new URL(`https://bahnhof.de/api/boards/${type}?evaNumbers=${evaNumber}`);
				break;
			case Profile.RIS:
				// remove last character ("s")
				const risType = type.slice(0, -1);
				apiUrl = new URL(`https://regio-guide.de/@prd/zupo-travel-information/api/public/ri/board/${risType}/${evaNumber}`);
				break;
			case Profile.HAFAS:
				const hafasType = type === RequestType.DEPARTURES ? "abfahrten" : "ankuenfte";
				apiUrl = new URL(`https://int.bahn.de/web/api/reiseloesung/${hafasType}`)
		}

		return apiUrl.toString();
	};
}

export { TimetableService, RequestType, Profile };
