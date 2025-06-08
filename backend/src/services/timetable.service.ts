import { t } from "elysia";
import { DateTimeObject } from "../types/datetime";
import { DateTime } from "luxon";
import { Message, SmallStop, Time, TimetableEntry } from "../models/core/models";
import { SingleTimetableEntrySchema } from "../models/elysia/timetable.model";
import { mapToProduct } from "../models/core/products";
import { extractJourneyNumber, extractLeadingLetters } from "../lib/regex";
import { database } from "../db/postgres";
import { stations } from "../db/core.schema";
import { eq, inArray } from "drizzle-orm";

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

		return await Promise.all(
			Object.values(response?.items)
				.filter((journeyRaw: any) => journeyRaw?.train?.journeyId)
				.map(async (journeyRaw: any) => ({
					entries: [
						await this.mapToJourney(journeyRaw, {
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
								this.mapToJourney(connectionRaw, {
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
						const matchingStops = query.filter(stop => stop.name === viaStop);
						if (matchingStops.length > 0) {
							const stopWithHighestWeight = matchingStops.reduce((prev, current) =>
								(current.weight > prev.weight) ? current : prev, matchingStops[0]);
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
					await this.mapToJourney(entry, {
						isBahnhofProfile: false,
						isDeparture: type === RequestType.DEPARTURES
					})
				]
			}))
		);
	};

	private mapToJourney = async (
		entry: any,
		options: {
			isBahnhofProfile: boolean;
			isDeparture: boolean;
		} = { isBahnhofProfile: false, isDeparture: true }
	): Promise<typeof SingleTimetableEntrySchema.static> => {
		const journeyId = entry?.journeyID ?? entry?.train?.journeyId ?? entry?.journeyId;
		const isHAFAS = journeyId?.startsWith("2|#") ?? false;

		const getOrigin = async (): Promise<SmallStop> => {
			if (isHAFAS) {
				const result = (
					await database
						.select({ name: stations.name })
						.from(stations)
						.where(eq(stations.evaNumber, entry?.originEvaNumber))
				)[0];

				return {
					name: result?.name ?? "NaN",
					evaNumber: entry?.originEvaNumber,
					cancelled: false
				} as SmallStop;
			}
			if (options.isBahnhofProfile)
				return options.isDeparture ? this.mapToSmallStop(entry?.stopPlace) : this.mapToSmallStop(entry?.origin);
			return options.isDeparture ? this.mapToSmallStop(entry?.station) : this.mapToSmallStop(entry?.origin);
		};
		const getDestination = async (): Promise<SmallStop> => {
			if (isHAFAS) {
				const result = (
					await database
						.select({ name: stations.name })
						.from(stations)
						.where(eq(stations.evaNumber, entry?.destinationEvaNumber))
				)[0];

				return {
					name: result?.name ?? "NaN",
					evaNumber: entry?.destinationEvaNumber,
					cancelled: false
				} as SmallStop;
			}
			if (options.isBahnhofProfile)
				return options.isDeparture ? this.mapToSmallStop(entry?.destination) : this.mapToSmallStop(entry?.stopPlace);
			return options.isDeparture ? this.mapToSmallStop(entry?.destination) : this.mapToSmallStop(entry?.station);
		};
		const isCancelled = () => {
			if (isHAFAS) return (entry?.meldungen ?? []).some((message: any) => message.type === "HALT_AUSFALL");
			return entry?.canceled ?? false;
		};

		const journey = {
			ris_journeyId: !isHAFAS ? journeyId : undefined,
			hafas_journeyId: isHAFAS ? journeyId : undefined,
			origin: await getOrigin(),
			destination: await getDestination(),
			cancelled: isCancelled(),
			timeInformation: this.mapToTime(entry),
			lineInformation: {
				productType: mapToProduct(entry?.type ?? entry?.train?.type ?? entry?.verkehrmittel?.produktGattung),
				productName: options.isBahnhofProfile
					? extractLeadingLetters(entry?.lineName)
					: (entry?.train?.category ?? entry?.verkehrmittel?.kurzText),
				journeyNumber: options.isBahnhofProfile
					? undefined
					: (entry?.train?.no ?? extractJourneyNumber(entry?.verkehrmittel?.name)),
				journeyName:
					entry?.lineName ??
					entry?.verkehrmittel?.mittelText ??
					entry?.train?.category + " " + entry?.train?.lineName,
				additionalJourneyName: !options.isBahnhofProfile ? undefined : entry?.additionalLineName,
				operator: options.isBahnhofProfile
					? undefined
					: {
							code: entry?.administration?.id,
							name: entry?.administration?.operatorName
						}
			},
			viaStops: (entry?.viaStops ?? entry?.ueber ?? []).map((rawStop: any) => this.mapToSmallStop(rawStop)),
			messages: !options.isBahnhofProfile
				? []
				: entry?.messages?.common
						.concat(entry?.messages?.delay)
						.concat(entry?.messages?.cancelation)
						.concat(entry?.messages?.destination)
						.concat(entry?.messages?.via)
						.map((messageRaw: any) => this.mapMessage(messageRaw))
		} as typeof SingleTimetableEntrySchema.static;

		if (isHAFAS) journey.messages = (entry?.meldungen ?? []).map((message: any) => this.mapMessage(message, true));
		if (journey.viaStops?.length === 0) journey.viaStops = undefined;
		return journey;
	};

	private mapToSmallStop = (stop: any): SmallStop => ({
		name: stop?.name,
		evaNumber: Number(stop?.evaNumber ?? stop?.evaNo),
		cancelled: stop?.canceled ?? false,
		additional: stop?.additional,
		separation: stop?.separation,
		nameParts:
			stop?.nameParts?.map((rawPart: any) => ({
				type: rawPart.type,
				value: rawPart.value
			})) ?? undefined
	});

	private mapToTime = (entry: any): Time => {
		const plannedTime = DateTime.fromISO(entry?.time ?? entry?.timeSchedule ?? entry?.zeit);
		let actualTime = DateTime.fromISO(entry?.timePredicted ?? entry?.timeDelayed ?? entry?.ezZeit);
		if (!actualTime.isValid) actualTime = plannedTime;

		const delay: number = actualTime.diff(plannedTime, ["seconds"]).seconds;

		const plannedPlatform = entry?.platform ?? entry?.gleis;
		let actualPlatform = entry?.platformPredicted ?? entry?.platformSchedule ?? entry?.ezGleis;
		if (!actualPlatform) actualPlatform = plannedPlatform;

		return {
			plannedTime: plannedTime.toISO(),
			actualTime: actualTime.toISO(),
			delay: delay,
			plannedPlatform: plannedPlatform,
			actualPlatform: actualPlatform
		} as Time;
	};

	private mapMessage = (entry: any, isHAFAS: boolean = false): Message => {
		if (isHAFAS) {
			const type = entry?.prioritaet === "HOCH" && entry?.type === "HALT_AUSFALL" ? "canceled-trip" : "general-warning";
			return {
				type: type,
				text: entry?.text
			} as Message;
		}

		return {
			type: entry?.type,
			text: entry?.text,
			links: entry?.links ?? undefined
		};
	};
}

export { TimetableService, RequestType, Profile };
