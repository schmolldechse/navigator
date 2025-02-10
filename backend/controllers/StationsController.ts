import { type Station } from "../models/station.ts";
import { mapToEnum, Products } from "../models/products.ts";
import { Controller, Get, Path, Query, Res, Route, type TsoaResponse } from "tsoa";
import { getRedisClient } from "../lib/redis.ts";

@Route("stations")
export class StationController extends Controller {
	@Get()
	async queryStations(
		@Query() query: string,
		@Res() badRequestResponse: TsoaResponse<400, { reason: string }>
	): Promise<Station[]> {
		if (!query) {
			return badRequestResponse(400, { reason: "Query parameter is required" });
		}

		const stations: Station[] = (await fetchStation(query))
			.filter((station: Station) => station.evaNumber);
		await cacheStations(stations);
		return stations;
	}

	@Get("/{evaNumber}")
	async getStationByEvaNumber(
		@Path() evaNumber: number,
		@Res() badRequestResponse: TsoaResponse<400, { reason: string }>
	): Promise<Station> {
		if (!Number.isInteger(evaNumber)) {
			return badRequestResponse(400, { reason: "EvaNumber is not an integer" });
		}

		const cachedStation = await getCachedStation(evaNumber.toString());
		if (cachedStation) return cachedStation;

		return (await fetchStation(evaNumber.toString()))[0];
	}
}

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
		throw new Error("HTTP Request error occurred");
	}

	const response = await request.json();
	if (!response || !Array.isArray(response)) {
		throw new Error("Invalid response format");
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
	const client = await getRedisClient();

	// check existing stations
	const keys = stations.map((stations: Station) => stations.evaNumber);
	const existingStations = await client.mGet(keys);

	// filter out existing stations
	const pipeline = client.multi();
	stations.map((station: Station) => {
		if (existingStations.includes(station.evaNumber)) return;
		const { coordinates, ...stationData } = station;
		pipeline.set(station.evaNumber, JSON.stringify(stationData));
	});

	await pipeline.exec();
}

const getCachedStation = async (evaNumber: string): Promise<Station | null> => {
	const cachedStation = await (await getRedisClient()).get(evaNumber);
	if (!cachedStation) return null;
	return JSON.parse(cachedStation!) as Station;
}