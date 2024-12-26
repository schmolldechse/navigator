import {NextRequest, NextResponse} from "next/server.js";
import {fromHafasLineId} from "@/app/lib/converter";

export async function GET(req: NextRequest) {
    const { searchParams } = new URL(req.url);
    const id = searchParams.get("id");

    if (!id) return NextResponse.json({ error: 'Id is required' }, { status: 400 });

    const lineData = await fromHafasLineId(id);
    if (!lineData) return NextResponse.json({ error: 'Line not found' });

    return NextResponse.json(lineData);
}
