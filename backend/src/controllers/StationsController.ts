import { Controller, Get, Path, Query, Res, Route, Tags, type TsoaResponse } from "tsoa";
import type { Station } from "../models/station.ts";
import { mapToEnum, Products } from "../models/products.ts";
import { connectToDb } from "../lib/db/mongo-data-db.ts";

@Route("stations")
@Tags("Stations")
export class StationController extends Controller {
	@Get()
	async queryStations(
		@Query() query: string,
		@Res() badRequestResponse: TsoaResponse<400, { reason: string }>
	): Promise<Station[]> {
		if (!query) {
			return badRequestResponse(400, { reason: "Query parameter is required" });
		}

		return await fetchAndCacheStations(query);
	}

	@Get("/{evaNumber}")
	async getStationByEvaNumber(
		@Path() evaNumber: string,
		@Res() badRequestResponse: TsoaResponse<400, { reason: string }>
	): Promise<Station> {
		if (!/^\d+$/.test(evaNumber)) {
			return badRequestResponse(400, { reason: "evaNumber is not an integer" });
		}

		const cachedStation = await getCachedStation(evaNumber.toString());
		if (cachedStation) return cachedStation;

		const stations = await fetchAndCacheStations(evaNumber.toString());
		if (!stations.length) return badRequestResponse(400, { reason: "Station not found" });

		return stations[0];
	}
}

const fetchAndCacheStations = async (searchTerm: string): Promise<Station[]> => {
	const stations: Station[] = (await fetchStation(searchTerm)).filter((station: Station) => station.evaNumber && /^\d+$/.test(station.evaNumber));
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

	return response.map((data: any) => ({
		name: data?.name,
		locationId: data?.locationId,
		evaNumber: data?.evaNr,
		coordinates: {
			latitude: data?.coordinates?.latitude,
			longitude: data?.coordinates?.longitude
		},
		products: (data?.products || [])
			.map((product: string) => mapToEnum(product))
			.filter((product: Products): product is Products => product !== undefined)
	}));
};

const cacheStations = async (stations: Station[]): Promise<void> => {
	const client = await connectToDb();
	const collection = client.collection<Station>("stations");

	const bulkOps = stations.map(station => ({
		updateOne: {
			filter: { evaNumber: station.evaNumber },
			update: { $setOnInsert: station },
			upsert: true
		}
	}));

	await collection.bulkWrite(bulkOps);
};

const getCachedStation = async (evaNumber: string): Promise<Station | null> => {
	const client = await connectToDb();
	const collection = client.collection<Station>("stations");

	return await collection.findOne({ evaNumber }, { projection: { _id: 0 } }) as Station;
};
