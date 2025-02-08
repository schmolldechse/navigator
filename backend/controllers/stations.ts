// @ts-types="npm:@types/express"
import express from "npm:express";
import { Station } from "../models/station.ts";
import { mapToEnum, Products } from "../models/products.ts";

export class StationController {
	public router = express.Router();

	constructor() {
		this.router.get("/api/v1/stations", this.handleRequest.bind(this));
	}

	async handleRequest(
		req: express.Request,
		res: express.Response,
	): Promise<void> {
		const { query } = req.query as unknown as { query: string | number };
		if (!query) {
			res.status(400).send("Either a 'evaNr' or a Station name is missing");
			return;
		}

		if (typeof query === "number") {
			const data = await fetchStation(query.toString());
			const station = data[0];

			res.status(200).json(station);
		} else if (typeof query === "string") {
			const data = await fetchStation(query);

			res.status(200).json(data);
		} else res.status(400).send("Invalid query type");
	}
}

const fetchStation = async (searchTerm: string): Promise<Station[]> => {
	const request = await fetch(
		"https://app.vendo.noncd.db.de/mob/location/search",
		{
			method: "POST",
			headers: {
				"Accept": "application/x.db.vendo.mob.location.v3+json",
				"Content-Type": "application/x.db.vendo.mob.location.v3+json",
				"X-Correlation-ID": crypto.randomUUID() + "_" +
					crypto.randomUUID(),
			},
			body: JSON.stringify({ locationTypes: ["ALL"], searchTerm }),
		},
	);

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
			longitude: data?.coordinates?.longitude,
		},
		products: (data?.products || [])
			.map((product: string) => mapToEnum(product))
			.filter((product: Products): product is Products => product !== undefined),
	}));
};
