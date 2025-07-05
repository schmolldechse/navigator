import { SingleTimetableEntrySchema } from "../models/elysia/timetable.model";
import { Time, TimetableEntry, TimetableMessage, TimetableStop } from "../models/core/models";
import { database } from "../db/postgres";
import { stations } from "../db/core.schema";
import { eq } from "drizzle-orm";
import { mapToProduct } from "../models/core/products";
import { extractJourneyNumber, extractLeadingLetters, normalize } from "../lib/regex";
import { DateTime } from "luxon";
import { RequestType } from "./timetable.service";

class TimetableHelper {
	mergeEntry = (
		entryA: typeof SingleTimetableEntrySchema.static,
		entryB: typeof SingleTimetableEntrySchema.static
	): typeof SingleTimetableEntrySchema.static => ({
		...entryB,
		...entryA,
		ris_journeyId: entryA.ris_journeyId ?? entryB.ris_journeyId,
		hafas_journeyId: entryA.hafas_journeyId ?? entryB.hafas_journeyId,
		origin: entryA.origin ?? entryB.origin,
		destination: entryA.destination ?? entryB.destination,
		timeInformation: entryA.timeInformation ?? entryB.timeInformation,
		lineInformation: {
			...entryB.lineInformation,
			...entryA.lineInformation,
			// prefer from entryB, as the RIS/ HAFAS entries are more complete
			journeyNumber: entryA?.lineInformation.journeyNumber ?? entryB.lineInformation.journeyNumber,
			operator: entryA?.lineInformation.operator ?? entryB.lineInformation.operator
		},
		viaStops: entryA.viaStops && entryA.viaStops.length > 1 ? entryA.viaStops : entryB.viaStops,
		messages: entryA.messages ? [...(entryA.messages ?? []), ...(entryB.messages ?? [])] : entryB.messages
	});

	// merge both timetables by preferring the entries from timetableA
	mergeTimetables = (
		timetableEntriesA: TimetableEntry[],
		timetableEntriesB: TimetableEntry[],
		type: RequestType = RequestType.DEPARTURES
	): TimetableEntry[] => {
		if (timetableEntriesA.length === 0) return timetableEntriesB;
		if (timetableEntriesB.length === 0) return timetableEntriesA;

		// simplify timetableB as the entries are nested
		let simplifiedTimetableEntriesB: (typeof SingleTimetableEntrySchema.static)[] = timetableEntriesB.flatMap(
			(entry) => entry.entries
		);
		timetableEntriesA.forEach((timetableA: TimetableEntry) => {
			timetableA.entries.map((singleTimetableA: typeof SingleTimetableEntrySchema.static) => {
				// search for a matching entry in simplifiedTimetableEntriesB
				const matchingEntry = simplifiedTimetableEntriesB.find(
					(singleTimetableB: typeof SingleTimetableEntrySchema.static) =>
						this.doesTimetableMatch(singleTimetableA, singleTimetableB, type)
				);
				if (matchingEntry) {
					// remove singleTimetableB from simplifiedTimetableEntriesB
					simplifiedTimetableEntriesB = simplifiedTimetableEntriesB.filter((entry) => entry !== matchingEntry);

					// merge the two entries
					Object.assign(singleTimetableA, this.mergeEntry(singleTimetableA, matchingEntry));
					return singleTimetableA;
				}

				// keep original entry from timetableA
				return singleTimetableA;
			});
		});

		// insert remaining entries from simplifiedTimetableEntriesB
		simplifiedTimetableEntriesB.forEach((singleTimetableB: typeof SingleTimetableEntrySchema.static) =>
			timetableEntriesA.push({ entries: [singleTimetableB] })
		);

		return timetableEntriesA;
	};

	doesTimetableMatch = (
		timetableA: typeof SingleTimetableEntrySchema.static,
		timetableB: typeof SingleTimetableEntrySchema.static,
		type: RequestType = RequestType.DEPARTURES
	): boolean => {
		if (timetableA.ris_journeyId === timetableB.ris_journeyId) return true;
		if (timetableA.hafas_journeyId === timetableB.hafas_journeyId) return true;

		const matchesLine = (): boolean =>
			normalize(timetableA.lineInformation.journeyName) === normalize(timetableB.lineInformation.journeyName) ||
			timetableA.lineInformation.journeyNumber === timetableB.lineInformation.journeyNumber;

		const matchesTime = (): boolean => {
			const aTime = DateTime.fromISO(timetableA.timeInformation.plannedTime);
			const bTime = DateTime.fromISO(timetableB.timeInformation.plannedTime);
			if (!aTime.isValid || !bTime.isValid) return false;
			return aTime.equals(bTime);
		};

		const matchesPlatform = (): boolean => {
			if (!timetableA.timeInformation.plannedPlatform || !timetableB.timeInformation.plannedPlatform) return true;
			return timetableA.timeInformation.plannedPlatform === timetableB.timeInformation.plannedPlatform;
		};

		const matchesStop = (): boolean => {
			if (type === RequestType.DEPARTURES) return timetableA.origin.evaNumber === timetableB.origin.evaNumber;
			return timetableA.destination.evaNumber === timetableB.destination.evaNumber;
		};

		return matchesLine() && matchesTime() && matchesPlatform() && matchesStop();
	};

	mapToJourney = async (
		entry: any,
		options: {
			isBahnhofProfile: boolean;
			isDeparture: boolean;
		} = { isBahnhofProfile: false, isDeparture: true }
	): Promise<typeof SingleTimetableEntrySchema.static> => {
		const journeyId = entry?.journeyID ?? entry?.train?.journeyId ?? entry?.journeyId;
		const isHAFAS = journeyId?.startsWith("2|#") ?? false;

		const getOrigin = async (): Promise<TimetableStop> => {
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
				} as TimetableStop;
			}
			if (options.isBahnhofProfile)
				return options.isDeparture ? this.mapToSmallStop(entry?.stopPlace) : this.mapToSmallStop(entry?.origin);
			return options.isDeparture ? this.mapToSmallStop(entry?.station) : this.mapToSmallStop(entry?.origin);
		};
		const getDestination = async (): Promise<TimetableStop> => {
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
				} as TimetableStop;
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
		return journey;
	};

	mapToSmallStop = (stop: any): TimetableStop => ({
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

	mapToTime = (entry: any): Time => {
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

	mapMessage = (entry: any, isHAFAS: boolean = false): TimetableMessage => {
		if (isHAFAS) {
			const type = entry?.prioritaet === "HOCH" && entry?.type === "HALT_AUSFALL" ? "canceled-trip" : "general-warning";
			return {
				type: type,
				text: entry?.text
			} as TimetableMessage;
		}

		return {
			type: entry?.type,
			text: entry?.text,
			links: entry?.links ?? undefined
		} as TimetableMessage;
	};
}

export { TimetableHelper };
