import { t } from "elysia";
import { DateTimeObject } from "../types/datetime";
import { DateTime } from "luxon";

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
}

export { TimetableService };
