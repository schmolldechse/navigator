import { type RequestType, retrieveBahnhofJourneys } from "./requests.ts";
import { Controller, Get, Queries, Route, Tags } from "tsoa";
import type { Journey } from "../../models/connection.ts";

class BahnhofQuery {
	evaNumber!: number;
	type!: RequestType;
	duration?: number = 60;
	locale?: string = "en";
}

@Route("timetable/bahnhof")
@Tags("Timetable")
export class BahnhofController extends Controller {
	/**
	 * Returns journeys from DeutscheBahn Bahnhof API. All journeys include RIS identifiers.
	 */
	@Get()
	async getBahnhofJourneys(@Queries() query: BahnhofQuery): Promise<Journey[]> {
		return await retrieveBahnhofJourneys(query);
	}
}
