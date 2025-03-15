import { Controller, Get, Queries, Route, Tags } from "tsoa";
import { DateTime } from "luxon";
import { HttpError } from "../../lib/errors/HttpError.ts";
import { mapToRoute } from "../../lib/mapping.ts";
import { type RouteData } from "../../models/route.ts";

class RoutePlannerQuery {
	/**
	 * @isInt evaNumber of a station
	 * @pattern /^\d+$/
	 * @example 8000096
	 */
	from!: number;
	/**
	 * @isInt evaNumber of a station
	 * @pattern /^\d+$/
	 * @example 8000096
	 */
	to!: number;
	departure?: string;
	arrival?: string;
	results?: number = 5;
	earlierThan?: string;
	laterThan?: string;
}

@Route("journey/route-planner")
@Tags("Journey")
export class JourneyController extends Controller {
	@Get()
	async getRouteByDefinition(@Queries() query: RoutePlannerQuery): Promise<RouteData> {
		if (query.to === query.from) throw new HttpError(400, "From and to cannot be the same station");

		if (!query.departure && !query.arrival && !query.earlierThan && !query.laterThan) throw new HttpError(400, "Missing either 'departure', 'arrival', 'earlierThan' or 'laterThan'");

		if (query.departure && query.arrival) throw new HttpError(400, "Either departure or arrival can be set, not both");
		if (query.earlierThan && query.laterThan) throw new HttpError(400, "Either earlierThan or laterThan can be set, not both");

		if (query.departure && !DateTime.fromISO(query.departure).isValid) throw new HttpError(400, "Invalid departure date");
		if (query.arrival && !DateTime.fromISO(query.arrival).isValid) throw new HttpError(400, "Invalid arrival date");

		return await fetchRoute(query);
	}
}

const fetchRoute = async (query: RoutePlannerQuery): Promise<RouteData> => {
	const params = new URLSearchParams({
		from: query.from.toString(),
		to: query.to.toString()
	});

	if (query.earlierThan) params.set("earlierThan", query.earlierThan);
	else if (query.laterThan) params.set("laterThan", query.laterThan);
	else {
		params.set("results", query.results!.toString());

		if (query.departure) params.set("departure", query.departure);
		else if (query.arrival) params.set("arrival", query.arrival);
	}

	const request = await fetch(`https://vendo-prof-db.voldechse.wtf/journeys?${params.toString()}`, { method: "GET", });
	if (!request.ok) throw new Error("Failed to fetch route");

	return await mapToRoute(await request.json()) as RouteData;
};
