import {getRedisClient} from "@/app/lib/redis";
import {NextRequest, NextResponse} from "next/server.js";
import {v4} from "uuid";
import {Station} from "@/app/lib/objects";

export async function POST(req: NextRequest) {
    const body = await req.json();
    const {query} = body;

    if (!query) return NextResponse.json({error: 'Either a evaNr or station name is missing'}, {status: 400});

    const fetchStation = async (searchTerm: string): Promise<Station[]> => {
        const request = await fetch("https://app.vendo.noncd.db.de/mob/location/search", {
            method: 'POST',
            headers: {
                "Accept": "application/x.db.vendo.mob.location.v3+json",
                "Content-Type": "application/x.db.vendo.mob.location.v3+json",
                "X-Correlation-ID": v4() + "_" + v4()
            },
            body: JSON.stringify({locationTypes: ["ALL"], searchTerm: searchTerm})
        });
        if (!request.ok) throw new Error("HTTP Request error occurred");

        const response = await request.json();
        if (!response || !Array.isArray(response)) throw new Error("Invalid response format");

        return response.map((data: any) => ({
            name: data.name,
            locationId: data.locationId,
            evaNr: data.evaNr,
            coordinates: {
                latitude: data.coordinates.latitude,
                longitude: data.coordinates.longitude
            },
            products: data.products
        }))
    }

    if (typeof query === "number") {
        const redis = await getRedisClient();
        const cached = await redis.get(String(query));
        if (cached) return NextResponse.json(JSON.parse(cached), {status: 200});

        try {
            const data = await fetchStation(String(query));

            const station = data[0];
            await redis.set(String(query), JSON.stringify(station));

            return NextResponse.json(station, {status: 200});
        } catch (error) {
            return NextResponse.json({error: error.message}, {status: 400});
        }
    }

    if (typeof query === "string") {
        try {
            const data = await fetchStation(query);
            return NextResponse.json({entries: data}, {status: 200});
        } catch (error) {
            return NextResponse.json({error: error.message}, {status: 400});
        }
    }

    return NextResponse.json({error: "'query' must be either a string or a number"}, {status: 400});
}
