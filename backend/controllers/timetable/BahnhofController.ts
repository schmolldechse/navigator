import { type RequestType, retrieveBahnhofJourneys } from "./requests.ts";
import { Controller, Get, Queries, Route } from "tsoa";
import type { Journey } from "../../models/connection.ts";

class BahnhofQuery {
	evaNumber!: string;
	type!: RequestType;
	duration?: number = 60;
	locale?: string = "en";
}

@Route("timetable/bahnhof")
export class BahnhofController extends Controller {
	@Get()
	async getBahnhofJourneys(@Queries() query: BahnhofQuery): Promise<Journey[]> {
		return await retrieveBahnhofJourneys(query);
	}
}
