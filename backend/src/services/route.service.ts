import { t } from "elysia";
import { HttpStatus } from "../response/status";
import { HttpError } from "../response/error";
import { DateTimeObject } from "../types/datetime";
import { DateTime } from "luxon";
import { mapToProduct, Products } from "../models/core/products";
import {
	ExtendedRouteStopSchema,
	NormalRouteSectionSchema,
	OccupancySchema,
	RouteDetailsSchema,
	RouteEntrySchema,
	RouteJourneyAttributeSchema,
	RouteMessageSchema,
	WalkingRouteSectionSchema
} from "../models/elysia/route.model";
import { database } from "../db/postgres";
import { stations } from "../db/core.schema";
import { eq } from "drizzle-orm";
import { TimeSchema } from "../models/elysia/time.model";

class RouteService {
	readonly routeBody = t.Object({
		from: t.Number({
			description: "The evaNumber of the station to start from",
			default: 0
		}),
		to: t.Number({
			description: "The evaNumber of the station to end at",
			default: 0
		}),
		type: t.UnionEnum(["departure", "arrival"], {
			default: "departure",
			description: "Whether to search based on departure or arrival time",
			enum: ["departure", "arrival"],
			error: () => {
				throw new HttpError(
					HttpStatus.HTTP_400_BAD_REQUEST,
					"Invalid type provided. Either `departure` or `arrival` is expected.`"
				);
			}
		}),
		when: DateTimeObject({
			fieldName: "when",
			default: DateTime.now().set({ second: 0, millisecond: 0 }).toISO(),
			description:
				"The date and time to search for routes, in ISO format (e.g., 2023-10-01T12:00:00Z). Defaults to the current date and time.",
			error: "Parameter 'endDate' could not be parsed as a date."
		}),
		filter: t.Object({
			disabledProducts: t.Array(
				t.String({
					description: "The name of the corresponding product to disable"
				}),
				{
					description: "List of disabled products",
					default: [],
					uniqueItems: true
				}
			),
			changeover: t.Optional(
				t.Object({
					maxAmount: t.Optional(
						t.Number({
							description: "Maximum allowed changeovers. -1 deactivates this.",
							default: -1
						})
					),
					minDuration: t.Optional(
						t.Number({
							description: "The minimum time required for the changeover in minutes. -1 deactivates this.",
							default: -1
						})
					)
				})
			),
			onlyFastRoutes: t.Optional(
				t.Boolean({
					description: "Whether to only return fast routes. Defaults to false.",
					default: false
				})
			)
		}),
		reference: t.Optional(t.String({ description: "The reference (either earlier/ later) to lookup routes for" }))
	});

	private readonly deutscheBahnOccupancies: (typeof OccupancySchema.static)[] = [
		{
			class: "FIRST_CLASS",
			level: 0,
			title: "Keine Auslastungsinformation verfügbar"
		},
		{
			class: "SECOND_CLASS",
			level: 0,
			title: "Keine Auslastungsinformation verfügbar"
		},
		{
			class: "FIRST_CLASS",
			level: 1,
			title: "Geringe Auslastung erwartet"
		},
		{
			class: "SECOND_CLASS",
			level: 1,
			title: "Geringe Auslastung erwartet"
		},
		{
			class: "FIRST_CLASS",
			level: 2,
			title: "Mittlere Auslastung erwartet",
			description:
				"Wir erwarten im Verlauf Ihrer Reise eine mittlere Auslastung. Reservieren Sie bereits jetzt Ihren Wunschplatz."
		},
		{
			class: "SECOND_CLASS",
			level: 2,
			title: "Mittlere Auslastung erwartet",
			description:
				"Wir erwarten im Verlauf Ihrer Reise eine mittlere Auslastung. Reservieren Sie bereits jetzt Ihren Wunschplatz."
		},
		{
			class: "FIRST_CLASS",
			level: 3,
			title: "Hohe Auslastung erwartet",
			description: "Wir erwarten im Verlauf Ihrer Reise eine hohe Auslastung. Wir empfehlen eine Sitzplatzreservierung."
		},
		{
			class: "SECOND_CLASS",
			level: 3,
			title: "Hohe Auslastung erwartet",
			description: "Wir erwarten im Verlauf Ihrer Reise eine hohe Auslastung. Wir empfehlen eine Sitzplatzreservierung."
		},
		{
			class: "FIRST_CLASS",
			level: 4,
			title: "Außergewöhnlich hohe Auslastung erwartet",
			description:
				"Wir erwarten im Verlauf Ihrer Reise eine außergewöhnlich hohe Auslastung in der 1. Klasse. Reisende, die noch keine Fahrkarte gekauft haben, wählen bitte eine andere Verbindung oder buchen ein Ticket der 2. Klasse."
		},
		{
			class: "SECOND_CLASS",
			level: 4,
			title: "Außergewöhnlich hohe Auslastung erwartet",
			description:
				"Wir erwarten im Verlauf Ihrer Reise eine außergewöhnlich hohe Auslastung in der 2. Klasse. Reisende, die noch keine Fahrkarte gekauft haben, wählen bitte eine andere Verbindung oder buchen ein Ticket der 1. Klasse."
		},
		{
			class: "FIRST_CLASS",
			level: 5,
			title: "Maximale Auslastung erwartet",
			description:
				"Wir erwarten im Verlauf Ihrer Reise eine maximale Auslastung. Reisende, die noch keine Fahrkarte gekauft haben, wählen bitte eine andere Verbindung."
		},
		{
			class: "SECOND_CLASS",
			level: 5,
			title: "Maximale Auslastung erwartet",
			description:
				"Wir erwarten im Verlauf Ihrer Reise eine maximale Auslastung. Reisende, die noch keine Fahrkarte gekauft haben, wählen bitte eine andere Verbindung."
		}
	];

	retrieveRoutes = async (body: typeof this.routeBody.static): Promise<typeof RouteDetailsSchema.static> => {
		const changeoverAmount = (): number | undefined => {
			const amount = body.filter?.changeover?.maxAmount;
			if (typeof amount !== "number" || isNaN(amount) || amount < 0) return undefined;
			return amount;
		};
		const changeoverTime = (): number | undefined => {
			const duration = body.filter?.changeover?.minDuration;
			if (typeof duration !== "number" || isNaN(duration) || duration < 0) return undefined;
			return duration;
		};

		const request = await fetch("https://www.bahn.de/web/api/angebote/fahrplan", {
			method: "POST",
			headers: {
				Accept: "application/json",
				"Content-Type": "application/json",
				"X-Correlation-ID": crypto.randomUUID() + "_" + crypto.randomUUID()
			},
			body: JSON.stringify({
				abfahrtsHalt: String(body.from),
				ankunftsHalt: String(body.to),
				anfrageZeitpunkt: body.when.toFormat("yyyy-MM-dd'T'HH:mm:ss"),
				ankunftSuche: body.type === "departure" ? "ABFAHRT" : "ANKUNFT",
				klasse: "KLASSE_2",
				produktgattungen: this.getAllowedProducts(body.filter.disabledProducts),
				reisende: [
					{
						typ: "ERWACHSENER",
						ermaessigungen: [
							{
								art: "KEINE_ERMAESSIGUNG",
								klasse: "KLASSENLOS"
							}
						],
						alter: [],
						anzahl: 1
					}
				],
				schnelleVerbindungen: body.filter?.onlyFastRoutes ?? false,
				sitzplatzOnly: false,
				bikeCarriage: false,
				reservierungsKontingenteVorhanden: false,
				nurDeutschlandTicketVerbindungen: false,
				deutschlandTicketVorhanden: false,
				maxUmstiege: changeoverAmount(),
				minUmstiegszeit: changeoverTime(),
				pagingReference: body.reference
			})
		});
		if (!request.ok)
			throw new HttpError(HttpStatus.HTTP_502_BAD_GATEWAY, `Routes could not be retrieved: ${request.statusText}`);

		const response = await request.json();
		if (!response.verbindungen || !Array.isArray(response.verbindungen))
			throw new HttpError(
				HttpStatus.HTTP_404_NOT_FOUND,
				`Response was expected to be an array, but got ${typeof response}`
			);

		return {
			earlierRef: response.verbindungReference.earlier,
			laterRef: response.verbindungReference.later,
			entries: await Promise.all(
				response.verbindungen.map(async (verbindungEntry: any) => await this.mapToRouteEntry(verbindungEntry))
			)
		} as typeof RouteDetailsSchema.static;
	};

	private isCancelled = (entry: any): boolean => {
		if (!(entry.risNotizen && entry.himMeldungen && entry.priorisierteMeldungen)) return false;
		if (
			!(
				Array.isArray(entry.risNotizen) &&
				Array.isArray(entry.himMeldungen) &&
				Array.isArray(entry.priorisierteMeldungen)
			)
		)
			return false;

		if (entry.risNotizen.find((risMessage: any) => risMessage.key === "text.realtime.stop.cancelled")) return true;
		if (entry.priorisierteMeldungen.find((prioMessage: any) => prioMessage.type === "HALT_AUSFALL")) return true;
		return false;
	};

	private mapMessages = (entry: any): (typeof RouteMessageSchema.static)[] => {
		if (!(entry.risNotizen && entry.himMeldungen && entry.priorisierteMeldungen)) return [];
		if (
			!(
				Array.isArray(entry.risNotizen) &&
				Array.isArray(entry.himMeldungen) &&
				Array.isArray(entry.priorisierteMeldungen)
			)
		)
			return [];

		const getType = (key: string): string => {
			switch (key) {
				case "text.realtime.connection.cancelled":
				case "text.realtime.stop.cancelled":
					return "cancelled";
				default:
					return "general-warning";
			}
		};

		return entry.priorisierteMeldungen
			.map((messageEntry: any) => {
				const risMessage = entry?.risNotizen.find((risMessage: any) => risMessage.value === messageEntry.text);
				if (risMessage)
					return {
						type: getType(risMessage.key),
						content: messageEntry.text
					} as typeof RouteMessageSchema.static;

				const himMessage = entry?.himMeldungen.find((himMessage: any) => himMessage.text === messageEntry.text);
				if (himMessage)
					return {
						type: "general-warning",
						content: messageEntry.text,
						modificationDate: himMessage?.modificationDate
							? DateTime.fromISO(himMessage.modificationDate).toISO()
							: undefined
					} as typeof RouteMessageSchema.static;

				// skip
				return null;
			})
			.filter(Boolean);
	};

	private mapOccupancies = (entry: any): (typeof OccupancySchema.static)[] => {
		if (!entry.auslastungsmeldungen) return [];
		if (!Array.isArray(entry.auslastungsmeldungen)) return [];

		return entry.auslastungsmeldungen
			.map((occupancy: any) => {
				const dbClass: string = occupancy.klasse === "KLASSE_1" ? "FIRST_CLASS" : "SECOND_CLASS";
				const dbLevel: number = occupancy.stufe === 99 ? 5 : occupancy.stufe;

				const matchedOccupancy = this.deutscheBahnOccupancies.find(
					(dbOccupancy) => dbOccupancy.class === dbClass && dbOccupancy.level === dbLevel
				);
				if (!matchedOccupancy) return null;
				return matchedOccupancy;
			})
			.filter(Boolean);
	};

	private mapToRouteEntry = async (entry: any): Promise<typeof RouteEntrySchema.static> => {
		const parseTimeSchema = (timeEntry: any, isDeparture: boolean = true): typeof TimeSchema.static => {
			const plannedTime = DateTime.fromISO(isDeparture ? timeEntry.abfahrtsZeitpunkt : timeEntry.ankunftsZeitpunkt);
			let actualTime = DateTime.fromISO(isDeparture ? timeEntry.ezAbfahrtsZeitpunkt : timeEntry.ezAnkunftsZeitpunkt);
			if (!actualTime.isValid) actualTime = plannedTime;

			const plannedPlatform: string = timeEntry?.gleis;
			let actualPlatform: string = timeEntry?.ezGleis;
			if (!actualPlatform) actualPlatform = plannedPlatform;

			return {
				plannedTime: plannedTime.toISO()!,
				actualTime: actualTime.toISO()!,
				delay: actualTime.diff(plannedTime, ["seconds"]).seconds,
				plannedPlatform: plannedPlatform,
				actualPlatform: actualPlatform
			};
		};

		const createWalkingSection = (section: any): typeof WalkingRouteSectionSchema.static => ({
			isWalking: true,
			distance: section.distanz,
			origin: {
				name: section.abfahrtsOrt,
				evaNumber: Number(section.abfahrtsOrtExtId),
				departure: parseTimeSchema(section, true)
			},
			destination: {
				name: section.ankunftsOrt,
				evaNumber: Number(section.ankunftsOrtExtId),
				arrival: parseTimeSchema(section, false)
			},
			cancelled: this.isCancelled(section),
			messages: this.mapMessages(section)
		});

		const createNormalSection = async (section: any): Promise<typeof NormalRouteSectionSchema.static> => {
			const buildStop = (
				stop: any,
				parseArrival: boolean = true,
				parseDeparture: boolean = true
			): typeof ExtendedRouteStopSchema.static => ({
				name: stop.name,
				evaNumber: Number(stop.extId),
				arrival: parseArrival ? (() => parseTimeSchema(stop, false))() : undefined,
				departure: parseDeparture ? (() => parseTimeSchema(stop, true))() : undefined,
				occupancy: this.mapOccupancies(stop),
				cancelled: this.isCancelled(stop),
				messages: this.mapMessages(stop)
			});

			const buildAttributes = (attribute: any): (typeof RouteJourneyAttributeSchema.static)[] => {
				if (!Array.isArray(attribute)) return [];

				const getCategory = (category: string): string => {
					switch (category) {
						case "INFORMATION":
							return "INFORMATION";
						case "FAHRRADMITNAHME":
							return "BICYCLE-TRANSPORT";
						case "BARRIEREFREI":
							return "ACCESSIBILITY";
						default:
							return "INFORMATION";
					}
				};

				return attribute
					.filter((attr: any) => !(attr.kategorie === "BEFÖRDERER" || attr.key === "BEF"))
					.map(
						(attr: any) =>
							({
								category: getCategory(attr.kategorie),
								value: attr.value,
								sectionsNote: attr?.teilstreckenHinweis
							}) as typeof RouteJourneyAttributeSchema.static
					);
			};

			return {
				hafas_journeyId: section.journeyId,
				origin: buildStop(section.halte[0], false, true),
				destination: buildStop(section.halte[section.halte.length - 1], true, false),
				messages: this.mapMessages(section),
				journeysDirection: section.verkehrsmittel?.richtung
					? await (async () => {
							const evaNumber = await database
								.select({ evaNumber: stations.evaNumber })
								.from(stations)
								.where(eq(stations.name, section.verkehrsmittel.richtung));
							return {
								name: section.verkehrsmittel.richtung,
								evaNumber: evaNumber[0]?.evaNumber ?? NaN
							};
						})()
					: undefined,
				cancelled: this.isCancelled(entry),
				isWalking: false,
				occupancy: this.mapOccupancies(entry),
				viaStops: (section.halte as any[]).slice(1, -1).map((viaStop: any) => buildStop(viaStop)),
				attributes: buildAttributes(section.verkehrsmittel.zugattribute),
				lineInformation: {
					productType: mapToProduct(section.verkehrsmittel.produktGattung),
					productName: section.verkehrsmittel.kurzText,
					journeyName: section.verkehrsmittel.mittelText,
					journeyNumber: this.extractJourneyNumber(section.verkehrsmittel.nummer),
					operator: section.verkehrsmittel.zugattribute.find((attr: any) => attr.kategorie === "BEFÖRDERER")?.value ?? null
				}
			};
		};

		return {
			refreshToken: entry.ctxRecon,
			sections: await Promise.all(
				entry.verbindungsAbschnitte.map(async (section: any) => {
					if (section.verkehrsmittel.typ === "WALK") return createWalkingSection(section);
					else return await createNormalSection(section);
				})
			),
			isAlternative: entry.isAlternativeVerbindung,
			occupancy: this.mapOccupancies(entry),
			messages: this.mapMessages(entry)
		} as typeof RouteEntrySchema.static;
	};

	private extractJourneyNumber = (entry: string): number => {
		const digits = entry.match(/\d/g);
		if (!digits) return NaN;
		return parseInt(digits.join(""), 10);
	}

	private getAllowedProducts = (disabledProducts: string[]): string[] => {
		disabledProducts = disabledProducts.map((product) => product.toUpperCase());
		return Object.values(Products)
			.filter((product: Products) => product !== Products.UNKNOWN && !disabledProducts.includes(product.toUpperCase()))
			.map((product: Products) => this.mapToBahnWebProduct(product));
	}

	/**
	 * Maps a product from the Products enum to the bahn.de API product string.
	 * @param product
	 */
	private mapToBahnWebProduct = (product: string): string => {
		if (!product) return "UNKNOWN";

		switch (product) {
			case Products.HOCHGESCHWINDIGKEITSZUEGE:
				return "ICE";
			case Products.INTERCITYUNDEUROCITYZUEGE:
				return "EC_IC";
			case Products.INTERREGIOUNDSCHNELLZUEGE:
				return "IR";
			case Products.NAHVERKEHRSONSTIGEZUEGE:
				return "REGIONAL";
			case Products.SBAHNEN:
				return "SBAHN";
			case Products.BUSSE:
				return "BUS";
			case Products.SCHIFFE:
				return "SCHIFF";
			case Products.UBAHN:
				return "UBAHN";
			case Products.STRASSENBAHN:
				return "TRAM";
			case Products.ANRUFPFLICHTIGEVERKEHRE:
				return "ANRUFPFLICHTIG";
			default:
				return "UNKNOWN";
		}
	};
}

export { RouteService };
