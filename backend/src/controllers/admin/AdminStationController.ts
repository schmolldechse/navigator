import { Controller, Get, Path, Post, Route, Security, Tags } from "tsoa";
import { getCollection } from "../../lib/db/mongo-data-db.ts";
import { DateTime } from "luxon";
import type { Station } from "../../models/station.ts";

@Route("admin/station")
@Tags("Admin")
@Security("better_auth", ["admin"])
export class AdminStationController extends Controller {

	@Get("query")
	async getEnabledQueryStations(): Promise<Station[]> {
		const collection = await getCollection("stations");
		return (await collection.find({ queryingEnabled: true }).toArray())
			.map(({ _id, lastQueried, queryingEnabled, ...rest }) => rest as Station);
	}

	// TODO: toggle queryingEnabled
	@Post("query/{evaNumber}")
	async updateLastQueried(
		@Path() evaNumber: number
	): Promise<{ ok: boolean }> {
		const collection = await getCollection("stations");
		const station = await collection.findOne({ evaNumber });
		if (!station) return { ok: false };

		station.lastQueried = DateTime.now();
		station.queryingEnabled = true;
		await collection.updateOne({ evaNumber }, { $set: station });

		return { ok: true };
	}
}
