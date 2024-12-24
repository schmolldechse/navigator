import {NextRequest, NextResponse} from "next/server.js";
import {fromHafasLineId} from "@/app/lib/converter";

export async function GET(req: NextRequest) {
    const { searchParams } = new URL(req.url);
    const lineName = searchParams.get("name");
    const operatorName = searchParams.get("operator");

    if (!lineName) return NextResponse.json({ success: false, error: 'Line name is required' }, { status: 400 });
    if (!operatorName) return NextResponse.json({ success: false, error: 'Operator name is required' }, { status: 400 });

    const lineData = await fromHafasLineId(lineName, operatorName);
    if (!lineData) return NextResponse.json({ success: false, error: 'Line not found' }, { status: 404 });

    return NextResponse.json(lineData);
}
