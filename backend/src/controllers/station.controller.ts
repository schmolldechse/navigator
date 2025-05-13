import Elysia, { t } from "elysia";
import { StationService } from "../services/station.service";
import { HttpStatus } from "../response/status";
import { type Station } from "navigator-core/src/models/station";
import { HttpError } from "../response/error";
import { DateTime } from "luxon";
import { StatistisService } from "../services/statistis.service";
import { DateTimeObject } from "../types/datetime";

const stationService = new StationService();
const statisticsService = new StatistisService();

const stationController = new Elysia({ prefix: "/station", tags: ["Stations"] })
	.decorate({ httpStatus: HttpStatus })
	.get("/", ({ query }) => stationService.fetchAndCacheStations(query.query), {
		query: t.Object({
			query: t.String()
		}),
		detail: {
			summary: "Query stations",
			description: "Search for stations by name. Returns a list of stations matching the search term."
		}
	})
	.get(
		"/:evaNumber",
		async ({ params: { evaNumber } }) => {
			const cachedStation = await stationService.getCachedStation(evaNumber);
			if (cachedStation) {
				const { _id, lastQueried, queryingEnabled, ...extracted } = cachedStation;
				return extracted as Station;
			}

			const stations = await stationService.fetchAndCacheStations(String(evaNumber));
			if (!stations.length) throw new HttpError(HttpStatus.HTTP_400_BAD_REQUEST, "Station not found");

			return stations[0];
		},
		{
			params: t.Object({
				evaNumber: t.Number({ error: "Parameter 'evaNumber' is required and must be a number." })
			}),
			detail: {
				summary: "Station by evaNumber",
				description:
					"Get a station by its evaNumber. Returns the station object if available or fetches and caches it if not found locally."
			}
		}
	)
	.post(
		"/stats/:evaNumber",
		async ({ params: { evaNumber }, body }) => {
			body.startDate = (body.startDate || statisticsService.START_DATE).startOf("day");
			body.endDate = (body.endDate || DateTime.now()).endOf("day");
			if (body.startDate > body.endDate) throw new HttpError(400, "Start date can't be after end date");

			const cachedStation = await stationService.getCachedStation(evaNumber);
			if (!cachedStation) throw new HttpError(400, "Station not found");

			const evaNumbers = await stationService.getRelatedEvaNumbers(cachedStation);
			if (evaNumbers.length === 0) throw new HttpError(400, "Station not found");

			return evaNumbers;
		},
		{
			detail: {
				summary: "Get statistics for a station",
				description: "Get statistics for a station by its evaNumber."
			},
			params: t.Object({
				evaNumber: t.Number({ error: "Parameter 'evaNumber' is required and must be a number." })
			}),
			body: t.Object({
				startDate: t.Optional(
					DateTimeObject({
						fieldName: "startDate",
						default: statisticsService.START_DATE.toFormat("yyyy-MM-dd"),
						description: "Start date of the filter",
						error: "Parameter 'startDate' could not be parsed as a date."
					})
				),
				endDate: t.Optional(
					DateTimeObject({
						fieldName: "endDate",
						default: DateTime.now().toFormat("yyyy-MM-dd"),
						description: "End date of the filter",
						error: "Parameter 'endDate' could not be parsed as a date."
					})
				),
				filter: t.Object({
					delayThreshold: t.Number({
						default: 60,
						description: "Defines the threshold in seconds after which a delay is considered",
						minimum: 0,
						maximum: Number.MAX_SAFE_INTEGER
					}),
					products: t.Array(t.String(), { default: [], description: "The products to look for" }),
					lineName: t.Array(t.String(), {
						default: [],
						description: "The lineName to look for, e.g. ´MEX 12´"
					}),
					lineNumber: t.Array(t.String(), {
						default: [],
						description: "The exact lineNumber to look for, e.g. ´19974´"
					})
				})
			})
		}
	);
export default stationController;
