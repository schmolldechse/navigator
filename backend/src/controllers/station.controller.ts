import Elysia, { t } from "elysia";
import { StationService } from "../services/station.service";
import { HttpStatus } from "../response/status";
import { HttpError } from "../response/error";
import { DateTime } from "luxon";
import { StatisticsService } from "../services/statistics.service";
import { StationSchema } from "../models/elysia/station.model";
import { StopAnalyticsSchema } from "../models/elysia/analytics.model";

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
			const station = await stationService.fetchStationByEvaNumber(evaNumber);
			if (!station) throw new HttpError(HttpStatus.HTTP_400_BAD_REQUEST, "Station not found");

			return station;
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

			// filter out empty strings
			body.filter.products = body.filter.products.filter((product: string) => product.trim() !== "");
			body.filter.lineName = body.filter.lineName.filter((line: string) => line.trim() !== "");
			body.filter.lineNumber = body.filter.lineNumber.filter((line: string) => line.trim() !== "");

			const evaNumbers = await stationService.getRelatedEvaNumbers(evaNumber);
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
