import { t } from "elysia";
import { DateTimeObject } from "../types/datetime";
import { DateTime } from "luxon";
import { TimetableEntry } from "../models/core/models";
import { SingleTimetableEntrySchema } from "../models/elysia/timetable.model";
import { database } from "../db/postgres";
import { stations } from "../db/core.schema";
import { inArray } from "drizzle-orm";
import { TimetableHelper } from "./timetable.helper";

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
	private readonly helper: TimetableHelper = new TimetableHelper();

	retrieveCombined = async (
		evaNumber: number,
		type: RequestType = RequestType.DEPARTURES,
		queryParams: typeof this.query.static
	): Promise<TimetableEntry[]> => {
		const now: DateTime = DateTime.now().set({ second: 0, millisecond: 0 });
		const endTime: DateTime = queryParams.when.plus({ minute: queryParams.duration });

		// fallback to HAFAS & RIS if the requested time is in the past
		if (now >= endTime) {
			const [ris, hafas] = await Promise.all([
				this.retrieveRISConnections(evaNumber, type, queryParams),
				this.retrieveHafasConnections(evaNumber, type, queryParams)
			]);
			return this.helper.mergeTimetables(ris, hafas, type);
		}

		const remainingDuration: number = Math.ceil(endTime.diff(now, ["minute"]).minutes);
		const bahnhofDuration = Math.min(remainingDuration, 360);
		// fallback to HAFAS & RIS
		if (bahnhofDuration < 1) {
			const [ris, hafas] = await Promise.all([
				this.retrieveRISConnections(evaNumber, type, queryParams),
				this.retrieveHafasConnections(evaNumber, type, queryParams)
			]);
			return this.helper.mergeTimetables(ris, hafas, type);
		}

		// use Bahnhof, RIS & HAFAS
		const [bahnhof, ris, hafas] = await Promise.all([
			this.retrieveBahnhofConnections(evaNumber, type, { ...queryParams, duration: bahnhofDuration }),
			this.retrieveRISConnections(evaNumber, type, queryParams),
			this.retrieveHafasConnections(evaNumber, type, queryParams)
		]);

		let mergedTimetables: TimetableEntry[] = this.helper.mergeTimetables(
			bahnhof,
			this.helper.mergeTimetables(ris, hafas, type),
			type
		);
		// remove entries before & after the requested time
		mergedTimetables = mergedTimetables.filter((entry) => {
			const firstEntryTime = DateTime.fromISO(entry.entries[0].timeInformation.plannedTime);
			const lastEntryTime = DateTime.fromISO(entry.entries[entry.entries.length - 1].timeInformation.plannedTime);
			return firstEntryTime >= queryParams.when && lastEntryTime <= endTime;
		});

		// sort by actualTime
		mergedTimetables.sort((a, b) => {
			const aTime = DateTime.fromISO(a.entries[0].timeInformation.actualTime);
			const bTime = DateTime.fromISO(b.entries[0].timeInformation.actualTime);
			return aTime < bTime ? -1 : 1;
		});
		return mergedTimetables;
	};

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

		return await Promise.all(
			Object.values(response?.items)
				.filter((journeyRaw: any) => journeyRaw?.train?.journeyId)
				.map(async (journeyRaw: any) => ({
					entries: [
						await this.helper.mapToJourney(journeyRaw, {
							isBahnhofProfile: false,
							isDeparture: type === RequestType.DEPARTURES
						})
					]
				}))
		);
	};

	retrieveBahnhofConnections = async (
		evaNumber: number,
		type: RequestType = RequestType.DEPARTURES,
		queryParams: typeof this.query.static
	): Promise<TimetableEntry[]> => {
		let apiUrl: URL = new URL(`https://bahnhof.de/api/boards/${type}`);
		apiUrl.searchParams.append("evaNumbers", String(evaNumber));
		apiUrl.searchParams.append("duration", String(queryParams.duration));
		apiUrl.searchParams.append("locale", queryParams.language);

		const request = await fetch(apiUrl, { method: "GET" });
		if (!request.ok) return [];

		const response = await request.json();
		if (!response?.entries || !Array.isArray(response?.entries)) return [];

		return await Promise.all(
			Object.values(response?.entries)
				.filter(Array.isArray)
				.map(async (journeyRaw) => ({
					entries: await Promise.all(
						journeyRaw
							.filter((connectionRaw) => connectionRaw?.journeyID)
							.map((connectionRaw) =>
								this.helper.mapToJourney(connectionRaw, {
									isBahnhofProfile: true,
									isDeparture: type === RequestType.DEPARTURES
								})
							)
					)
				}))
		);
	};

	retrieveHafasConnections = async (
		evaNumber: number,
		type: RequestType = RequestType.DEPARTURES,
		queryParams: typeof this.query.static
	): Promise<TimetableEntry[]> => {
		const requestAmount = Math.ceil(queryParams.duration / 60);
		if (requestAmount === 1) return this.makeHafasRequest(evaNumber, type, queryParams);

		const requests = Array.from({ length: requestAmount }, (_, i) =>
			this.makeHafasRequest(evaNumber, type, {
				...queryParams,
				when: queryParams.when.plus({ minute: i * 60 }),
				duration: Math.min(60, queryParams.duration - i * 60)
			})
		);

		const results = (await Promise.all(requests)).flat();
		const uniqueJourneys = new Map<string, typeof SingleTimetableEntrySchema.static>();
		results.forEach((entry) => {
			entry.entries.forEach((journey) => {
				if (!journey.hafas_journeyId) return;
				if (uniqueJourneys.has(journey.hafas_journeyId)) return;
				uniqueJourneys.set(journey.hafas_journeyId, journey);
			});
		});

		// filter out journeys that are after the requested duration
		const endTime = queryParams.when.plus({ minute: queryParams.duration + 1 });
		for (const [id, journey] of uniqueJourneys) {
			const plannedTime = DateTime.fromISO(journey.timeInformation.actualTime);
			if (plannedTime > endTime) uniqueJourneys.delete(id);
		}

		// sort journeys by actualTime
		const sortedJourneys = Array.from(uniqueJourneys.values()).sort((a, b) => {
			const aTime = DateTime.fromISO(a.timeInformation.actualTime);
			const bTime = DateTime.fromISO(b.timeInformation.actualTime);
			return aTime < bTime ? -1 : 1;
		});

		return sortedJourneys.map((journey: typeof SingleTimetableEntrySchema.static) => ({
			entries: [journey]
		}));
	};

	private makeHafasRequest = async (
		evaNumber: number,
		type: RequestType = RequestType.DEPARTURES,
		queryParams: typeof this.query.static
	): Promise<TimetableEntry[]> => {
		const typeString = type === RequestType.DEPARTURES ? "abfahrten" : "ankuenfte";

		let apiUrl: URL = new URL(`https://int.bahn.de/web/api/reiseloesung/${typeString}`);
		apiUrl.searchParams.append("datum", queryParams.when.toFormat("yyyy-MM-dd"));
		apiUrl.searchParams.append("zeit", queryParams.when.toFormat("HH:mm:ss"));
		apiUrl.searchParams.append("ortExtId", String(evaNumber));
		apiUrl.searchParams.append("mitVias", String(true));

		const request = await fetch(apiUrl, { method: "GET" });
		if (!request.ok) return [];

		const response = await request.json();
		if (!response.entries || !Array.isArray(response.entries)) return [];

		const values = await Promise.all(
			Object.values(response.entries)
				.filter((entry: any) => entry?.journeyId)
				.map(async (entry: any) => {
					let viaStopsArray = entry?.ueber ?? [];
					if (viaStopsArray.length === 0) return { ...entry, ueber: [] };

					// gather all evaNumbers of the via stops
					const query = await database
						.select({ name: stations.name, evaNumber: stations.evaNumber, weight: stations.weight })
						.from(stations)
						.where(inArray(stations.name, viaStopsArray));

					viaStopsArray = viaStopsArray.map((viaStop: string) => {
						const matchingStops = query.filter((stop) => stop.name === viaStop);
						if (matchingStops.length > 0) {
							const stopWithHighestWeight = matchingStops.reduce(
								(prev, current) => (current.weight > prev.weight ? current : prev),
								matchingStops[0]
							);
							return { name: viaStop, evaNumber: stopWithHighestWeight.evaNumber };
						}
						return { name: viaStop, evaNumber: -1 };
					});

					return {
						...entry,
						ueber: viaStopsArray.slice(1, -1),
						originEvaNumber: viaStopsArray[0]?.evaNumber!,
						destinationEvaNumber: viaStopsArray[viaStopsArray.length - 1]?.evaNumber!
					};
				})
		);

		return await Promise.all(
			values.map(async (entry) => ({
				entries: [
					await this.helper.mapToJourney(entry, {
						isBahnhofProfile: false,
						isDeparture: type === RequestType.DEPARTURES
					})
				]
			}))
		);
	};
}

export { TimetableService, Profile, RequestType };
