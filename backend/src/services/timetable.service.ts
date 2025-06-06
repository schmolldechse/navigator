import { t } from "elysia";
import { DateTimeObject } from "../types/datetime";
import { DateTime } from "luxon";
import { Message, SmallStop, Time, TimetableEntry } from "../models/core/models";
import { SingleTimetableEntrySchema } from "../models/elysia/timetable.model";
import { mapToProduct } from "../lib/products";
import { extractLeadingLetters } from "../lib/regex";

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
	): Promise<TimetableEntry[]> => {
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
		apiUrl.searchParams.append(
			"timeStart",
			queryParams.when.toUTC().toISO({ includeOffset: false, suppressMilliseconds: true })!
		);
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
			.filter((journeyRaw: any) => journeyRaw?.train?.journeyId)
			.map((journeyRaw: any) => ({ entries: [this.mapToJourney(journeyRaw)] }));
	};

	retrieveBahnhofConnections = async (
		evaNumber: number,
		type: RequestType = RequestType.DEPARTURES,
		queryParams: typeof this.query.static
	): Promise<TimetableEntry[]> => {
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
				entries: journeyRaw
					.filter((connectionRaw) => connectionRaw?.journeyID)
					.map((connectionRaw) => this.mapToJourney(connectionRaw, { isBahnhofProfile: true }))
			}));
	};

	private mapToJourney = (
		entry: any,
		options: {
			isBahnhofProfile: boolean;
		} = { isBahnhofProfile: false }
	): typeof SingleTimetableEntrySchema.static => {
		const journeyId = entry?.journeyID ?? entry?.train?.journeyId;
		const isHAFAS = journeyId?.startsWith("2|#") ?? false;

		return {
			ris_journeyId: !isHAFAS ? journeyId : undefined,
			hafas_journeyId: isHAFAS ? journeyId : undefined,
			origin: this.mapToSmallStop(entry.stopPlace ?? entry?.station),
			provenance: !isHAFAS ? undefined : undefined,
			destination: this.mapToSmallStop(entry?.destination),
			direction: !isHAFAS ? undefined : undefined,
			cancelled: entry?.canceled ?? false,
			timeInformation: this.mapToTime(entry),
			lineInformation: {
				productType: mapToProduct(entry?.type ?? entry?.train?.type).value,
				productName: options.isBahnhofProfile ? extractLeadingLetters(entry?.lineName) : entry?.train?.category,
				journeyNumber: options.isBahnhofProfile ? undefined : entry?.train?.no,
				journeyName: entry?.lineName ?? entry?.train?.category + " " + entry?.train?.lineName,
				additionalJourneyName: !options.isBahnhofProfile ? undefined : entry?.additionalLineName,
				operator: {
					code: entry?.administrationID ?? entry?.administration?.id,
					name: options.isBahnhofProfile ? undefined : entry?.administration?.operatorName
				}
			},
			viaStops: !options.isBahnhofProfile
				? undefined
				: (entry?.viaStops ?? []).map((rawStop: any) => this.mapToSmallStop(rawStop)),
			messages: !options.isBahnhofProfile
				? []
				: entry?.messages?.common
						.concat(entry?.messages?.delay)
						.concat(entry?.messages?.cancelation)
						.concat(entry?.messages?.destination)
						.concat(entry?.messages?.via)
						.map((messageRaw: any) => this.mapMessage(messageRaw))
		} as typeof SingleTimetableEntrySchema.static;
	};

	private mapToSmallStop = (stop: any): SmallStop => ({
		name: stop?.name,
		evaNumber: Number(stop?.evaNumber ?? stop?.evaNo),
		cancelled: stop?.canceled ?? false,
		additional: stop?.additional ? stop?.additional : undefined,
		separation: stop?.separation ? stop?.separation : undefined,
		nameParts:
			stop?.nameParts?.map((rawPart: any) => ({
				type: rawPart.type,
				value: rawPart.value
			})) ?? undefined
	});

	private mapToTime = (entry: any): Time => {
		const plannedTime = DateTime.fromISO(entry?.time ?? entry?.timeSchedule);
		let actualTime = DateTime.fromISO(entry?.timePredicted ?? entry?.timeDelayed);
		if (!actualTime.isValid) actualTime = plannedTime;

		const delay: number = actualTime.diff(plannedTime, ["seconds"]).seconds;

		return {
			plannedTime: plannedTime.toISO(),
			actualTime: actualTime.toISO(),
			delay: delay,
			plannedPlatform: entry?.platform,
			actualPlatform: entry?.platformPredicted ?? entry?.platformSchedule
		} as Time;
	};

	private mapMessage = (entry: any): Message => ({
		type: entry?.type,
		text: entry?.text,
		links: entry?.links ?? undefined
	});
}

export { TimetableService, RequestType, Profile };
