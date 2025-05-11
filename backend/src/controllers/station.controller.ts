import Elysia, { t } from "elysia";
import { StationService } from "../services/station.service";
import { HttpStatus } from "../response/status";
import { type Station } from "navigator-core/src/models/station";
import { HttpError } from "../response/error";

const service = new StationService();

const stationController = new Elysia({ prefix: "/station", tags: ["Stations"] })
	.decorate({ httpStatus: HttpStatus })
	.get("/", ({ query }) => service.fetchAndCacheStations(query.query), {
		query: t.Object({
			query: t.String({ required: true })
		}),
		detail: {
			summary: "Query stations",
			description: "Search for stations by name. Returns a list of stations matching the search term."
		}
	})
	.get(
		"/:evaNumber",
		async ({ params: { evaNumber } }) => {
			const cachedStation = await service.getCachedStation(evaNumber);
			if (cachedStation) {
				const { _id, lastQueried, queryingEnabled, ...extracted } = cachedStation;
				return extracted as Station;
			}

			const stations = await service.fetchAndCacheStations(String(evaNumber));
			if (!stations.length) throw new HttpError(HttpStatus.HTTP_400_BAD_REQUEST, "Station not found");

			return stations[0];
		},
		{
			params: t.Object({
				evaNumber: t.Number({ required: true })
			}),
			detail: {
				summary: "Station by evaNumber",
				description:
					"Get a station by its evaNumber. Returns the station object if available or fetches and caches it if not found locally."
			}
		}
	);

export default stationController;
