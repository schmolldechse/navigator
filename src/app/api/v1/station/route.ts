import { getRedisClient } from "@/app/lib/redis";
import {NextRequest, NextResponse} from "next/server.js";

export async function POST(req: NextRequest) {
    const body = await req.json();
    const { id } = body;

    if (!id) return NextResponse.json({ success: false, error: 'Station id is required' }, { status: 400 });

    const redis = await getRedisClient();
    const result = await redis.get(id);

    if (result) return NextResponse.json({ success: true, station: result });

    const request = await fetch(`https://vendo-prof-dbnav.voldechse.wtf/stops/${id}`);
    if (!request.ok) return NextResponse.json({ success: false, error: 'HTTP Request error occurred' }, { status: 400 });

    const data = await request.json();
    await redis.set(id, data.name);

    return NextResponse.json({ success: true, station: data.name }, { status: 200 });
}
