import { Controller, Get, Path, Query, Route, Tags } from "tsoa";
import type { Station } from "../models/station.ts";
import { mapToProduct, type Product, Products } from "../models/products.ts";
import { getCollection } from "../lib/db/mongo-data-db.ts";
import { HttpError } from "../lib/errors/HttpError.ts";
import type { StationDocument } from "../db/mongodb/station.schema.ts";

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
		if (cachedStation) return cachedStation;

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
	const request = await fetch("https://app.vendo.noncd.db.de/mob/location/search", {
		method: "POST",
		headers: {
			Accept: "application/x.db.vendo.mob.location.v3+json",
			"Content-Type": "application/x.db.vendo.mob.location.v3+json",
			"X-Correlation-ID": crypto.randomUUID() + "_" + crypto.randomUUID()
		},
		body: JSON.stringify({ locationTypes: ["ALL"], searchTerm })
	});

	if (!request.ok) {
		throw new Error(`Failed to fetch stations for ${searchTerm}`);
	}

	const response = await request.json();
	if (!response || !Array.isArray(response)) {
		throw new Error(`Response was expected to be an array, but got ${typeof response}`);
	}

	return response
		.filter((data: any) => /^\d+$/.test(data?.evaNr))
		.map((data: any) => ({
			name: data?.name,
			locationId: data?.locationId,
			evaNumber: Number(data?.evaNr),
			coordinates: {
				latitude: data?.coordinates?.latitude,
				longitude: data?.coordinates?.longitude
			},
			products: (data?.products || [])
				.map(mapToProduct)
				.filter((product: Product) => product != Products.UNKNOWN)
				.map((product: Product) => product.value)
		}));
};

const cacheStations = async (stations: Station[]): Promise<void> => {
	const collection = await getCollection("stations");

	const bulkOps = stations.map((station) => ({
		updateOne: {
			filter: { evaNumber: station.evaNumber },
			update: { $setOnInsert: station },
			upsert: true
		}
	}));

	await collection.bulkWrite(bulkOps);
};

const getCachedStation = async (evaNumber: number): Promise<Station | null> => {
	const collection = await getCollection("stations");
	const station = (await collection.findOne({ evaNumber })) as StationDocument;
	if (!station) return null;

	const { _id, lastQueried, queryingEnabled, ...extracted } = station;
	return extracted as Station;
};
