import { Controller, Get, Path, Query, Route, Tags } from "tsoa";
import type { Station } from "../../models/station.ts";
import { mapToProduct, Products } from "../../models/products.ts";
import { getCollection } from "../../lib/db/mongo-data-db.ts";
import { HttpError } from "../../lib/errors/HttpError.ts";
import { getCachedStation } from "./request.ts";

@Route("stations")
@Tags("Stations")
export class StationController extends Controller {
	/**
	 * Searches for stations from the Deutsche Bahn API on the specified query.
	 * @param query The possible station name / evaNumber
	 */
	@Get()
	async queryStations(@Query() query: string): Promise<Station[]> {
		return await fetchAndCacheStations(query);
	}

	/**
	 * Searches for a station exactly by its evaNumber.
	 * @param evaNumber The evaNumber of the station
	 */
	@Get("/{evaNumber}")
	async getStationByEvaNumber(@Path() evaNumber: number): Promise<Station> {
		const cachedStation = await getCachedStation(evaNumber);
		if (cachedStation) {
			const { _id, lastQueried, queryingEnabled, ...extracted } = cachedStation;
			return extracted as Station;
		}

		const stations = await fetchAndCacheStations(String(evaNumber));
		if (!stations.length) throw new HttpError(400, "Station not found");

		return stations[0];
	}
}

const fetchAndCacheStations = async (searchTerm: string): Promise<Station[]> => {
	const stations: Station[] = await fetchStation(searchTerm);
	if (stations.length > 0) await cacheStations(stations);

	return stations;
};

const fetchStation = async (searchTerm: string): Promise<Station[]> => {
	const params = new URLSearchParams({
		query: searchTerm,
		limit: "10"
	});
	const request = await fetch(`https://vendo-prof-db.voldechse.wtf/locations?${params.toString()}`, { method: "GET" });
	if (!request.ok) throw new Error(`Failed to fetch stations for ${searchTerm}`);

	const response = await request.json();
	if (!response || !Array.isArray(response))
		throw new Error(`Response was expected to be an array, but got ${typeof response}`);

	return response
		.filter((data: any) => /^\d+$/.test(data?.id))
		.map((data: any) => ({
			name: data?.name,
			evaNumber: Number(data?.id),
			coordinates: {
				latitude: data?.location?.latitude,
				longitude: data?.location?.longitude
			},
			// either "ril100Ids" is included directly in the object, or it is, for whatever reason, contained in a nested "station" object
			ril100: (data?.ril100Ids || data?.station?.ril100Ids || []).map((ril100Id: string) => ril100Id),
			products: Object.entries(data?.products || [])
				.filter(([key, value]) => value === true)
				.map(([key]) => mapToProduct(key))
				.filter((product) => product.value !== Products.UNKNOWN.value)
				.map((product) => product.value)
		}));
};

const cacheStations = async (stations: Station[]): Promise<void> => {
	const collection = await getCollection("stations");

	const bulkOps = stations.map((station) => ({
		updateOne: {
			filter: { evaNumber: station.evaNumber },
			update: {
				$set: {
					name: station.name,
					coordinates: station.coordinates,
					ril100: station.ril100,
					products: station.products
				}
			},
			upsert: true
		}
	}));

	await collection.bulkWrite(bulkOps);
};
