import Elysia, { t } from "elysia";
import { StationService } from "../services/station.service";
import { HttpStatus } from "../response/status";
import { type Station, StationSchema, StopAnalyticsSchema } from "navigator-core/src/models/station";
import { HttpError } from "../response/error";
import { DateTime } from "luxon";
import { StatisticsService } from "../services/statistics.service";

const stationService = new StationService();
const statisticsService = new StatisticsService();

const stationController = new Elysia({ prefix: "/station", tags: ["Stations"] })
	.decorate({ httpStatus: HttpStatus })
	.get("/", ({ query }) => stationService.fetchStations(query.query), {
		query: t.Object({
			query: t.String()
		}),
		detail: {
			summary: "Query stations",
			description: "Search for stations by name. Returns a list of stations matching the search term."
		},
		response: t.Array(StationSchema)
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
			},
			response: StationSchema
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

			return await statisticsService.startEvaluation(body, evaNumbers);
		},
		{
			detail: {
				summary: "Get statistics for a station",
				description: "Get statistics for a station by its evaNumber."
			},
			params: t.Object({
				evaNumber: t.Number({ error: "Parameter 'evaNumber' is required and must be a number." })
			}),
			body: statisticsService.statsBody,
			response: StopAnalyticsSchema
		}
	);
export default stationController;
