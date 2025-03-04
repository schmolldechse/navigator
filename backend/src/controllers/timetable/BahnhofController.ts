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
@Tags("Bahnhof")
export class BahnhofController extends Controller {
	@Get()
	async getBahnhofJourneys(@Queries() query: BahnhofQuery): Promise<Journey[]> {
		return await retrieveBahnhofJourneys(query);
	}
}
