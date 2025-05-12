import Elysia, { t } from "elysia";
import { StationService } from "../services/station.service";
import { HttpStatus } from "../response/status";
import { type Station } from "navigator-core/src/models/station";
import { HttpError } from "../response/error";
import { DateTime } from "luxon";

const service = new StationService();

const START_DATE = DateTime.fromObject({ day: 17, month: 12, year: 2024 }).startOf("day");

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
	)
	.post("/stats/:evaNumber", ({ params: { evaNumber } }) => {}, {
		body: t.Object({
			startDate: t.Date({ default: START_DATE.toFormat("yyyy-MM-dd"), description: "Start date of the filter" }),
			endDate: t.Date({ default: DateTime.now().toFormat("yyyy-MM-dd"), description: "End date of the filter" }),
			filter: t.Object({
				delayThreshold: t.Number({
					default: 60,
					description: "Defines the threshold in seconds after which a delay is considered",
					minimum: 0,
					maximum: Number.MAX_SAFE_INTEGER
				}),
				products: t.Array(t.String(), { default: [], description: "The products to look for" }),
				lineName: t.Array(t.String(), { default: [], description: "The lineName to look for, e.g. ´MEX 12´" }),
				lineNumber: t.Array(t.String(), {
					default: [],
					description: "The exact lineNumber to look for, e.g. ´19974´"
				})
			})
		})
	});
export default stationController;
