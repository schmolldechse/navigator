import {Station} from "../../models/station.ts";
import {mapToEnum, Products} from "../../models/products.ts";
import { Context } from "@oak/oak";

export class StationController {
    async handleRequest(req: Context["request"]): Promise<Response> {
        if (req.method !== "POST") return new Response("Method not allowed", {status: 405});

        let body = req.body;
        const { query } = body;

        if (!query) return new Response("Either a 'evaNr' oder 'Station name' is missing", {status: 400});

        if (typeof query === "number") {
            const data = await fetchStation(query.toString());
            const station = data[0];

            return new Response(JSON.stringify(station), {status: 200});
        } else if (typeof query === "string") {
            const data = await fetchStation(query);
            return new Response(JSON.stringify(data), {status: 200});
        } else return new Response("Invalid query", {status: 400});
    }
}

const fetchStation = async (searchTerm: string): Promise<Station[]> => {
    const request = await fetch("https://app.vendo.noncd.db.de/mob/location/search", {
        method: "POST",
        headers: {
            "Accept": "application/x.db.vendo.mob.location.v3+json",
            "Content-Type": "application/x.db.vendo.mob.location.v3+json",
            "X-Correlation-ID": crypto.randomUUID() + "_" + crypto.randomUUID(),
        },
        body: JSON.stringify({ locationTypes: ["ALL"], searchTerm }),
    });

    if (!request.ok) {
        throw new Error("HTTP Request error occurred");
    }

    const response = await request.json();
    if (!response || !Array.isArray(response)) {
        throw new Error("Invalid response format");
    }

    return response.map((data: any) => ({
        name: data.name,
        locationId: data.locationId,
        evaNr: data.evaNr,
        coordinates: {
            latitude: data.coordinates.latitude,
            longitude: data.coordinates.longitude,
        },
        products: (data.products || [])
            .map((product: string) => mapToEnum(product))
            .filter((product: Products): product is Products => product !== undefined),
    }));
};
