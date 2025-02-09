import { type Station } from "../models/station.ts";
import { mapToEnum, Products } from "../models/products.ts";
import { Controller, Get, Query, Res, Route, type TsoaResponse } from "tsoa";

@Route("stations")
export class StationController extends Controller {
	@Get()
	async handleRequest(
		@Query() query: string,
		@Res() badRequestResponse: TsoaResponse<400, { reason: string }>
	): Promise<Station | Station[]> {
		if (!query) {
			return badRequestResponse(400, { reason: "Query parameter is required" });
		}

		if (isInteger(query)) {
			const data = await fetchStation(query.toString());
			return data[0];
		}

		return await fetchStation(query);
	}
}

const isInteger = (value: string): boolean => {
	return /^\d+$/.test(value);
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
