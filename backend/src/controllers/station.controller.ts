import Elysia, { t } from "elysia";
import { StationService } from "../services/station.service";
import { HttpStatus } from "../response/status";
import { HttpError } from "../response/error";
import { StationSchema } from "../models/elysia/station.model";

const stationService = new StationService();

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
	);

export default stationController;
