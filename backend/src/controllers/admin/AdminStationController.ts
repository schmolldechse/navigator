import { Controller, Get, Path, Post, Route, Security, Tags } from "tsoa";
import { getCollection } from "../../lib/db/mongo-data-db.ts";
import type { Station } from "../../models/station.ts";
import { HttpError } from "../../lib/errors/HttpError.ts";
import type { StationDocument } from "../../db/mongodb/station.schema.ts";

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

	@Post("query/{evaNumber}")
	async toggleQueryEnabled(
		@Path() evaNumber: number
	): Promise<Station> {
		const collection = await getCollection("stations");
		const updated = await collection.findOneAndUpdate(
			{ evaNumber },
			[{ $set: { queryingEnabled: { $not: "$queryingEnabled" } } }],
			{ returnDocument: "after" }
		) as StationDocument;

		if (!updated) throw new HttpError(400, "Station not found");

		const { _id, lastQueried, queryingEnabled, ...rest } = updated;
		return rest as Station;
	}
}
