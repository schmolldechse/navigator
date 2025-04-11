import { Controller, Get, Queries, Route, Tags } from "tsoa";
import { DateTime } from "luxon";
import { HttpError } from "../../lib/errors/HttpError.ts";
import { mapToRoute } from "../../lib/mapping.ts";
import { type RouteData } from "../../models/route.ts";
import { fromLineName } from "../../lib/converter.ts";
import type { LineColor } from "../../models/connection.ts";

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

	/**
	 * @maxItems 10 maximum of 10 products can be disabled
	 * @uniqueItems Only unique values are allowed
	 */
	disabledProducts?: string[];

	departure?: string;
	arrival?: string;
	results?: number = 5;
	earlierThan?: string;
	laterThan?: string;
}

class LineColorQuery {
	/**
	 * The name of the line
	 * @example MEX 12 , mex-12 , mex-12;mex-18
	 */
	line!: string;
	/**
	 * The name of the operator
	 * @example db-regio-ag-baden-wurttemberg , db-regio-ag-baden-wurttemberg;sweg-bahn-stuttgart-gmbh
	 */
	hafasOperatorCode?: string;
}

@Route("journey")
@Tags("Journey")
export class JourneyController extends Controller {
	/**
	 * Retrieve line colors based on the provided line names & optional operator codes. It supports querying multiple lines & operators, separated by semicolons (";")
	 */
	@Get("color")
	async getColorByLinename(@Queries() query: LineColorQuery): Promise<LineColor[]> {
		const lines = query.line.split(";");
		const operators = query.hafasOperatorCode ? query.hafasOperatorCode.split(";") : [];

		// only use as many operators as we have lines
		const validOperators = operators.slice(0, lines.length);

		// set operator for each line to undefined, if no operator is given
		const pairs = lines.map((line, index) => ({ line, operator: validOperators[index] || undefined }));

		return (await Promise.all(pairs.map((pair) => fromLineName(pair.line, pair.operator)))).flat();
	}

	@Get("route-planner")
	async getRouteByDefinition(@Queries() query: RoutePlannerQuery): Promise<RouteData> {
		if (query.to === query.from) throw new HttpError(400, "From and to cannot be the same station");

		if (!query.departure && !query.arrival && !query.earlierThan && !query.laterThan)
			throw new HttpError(400, "Missing either 'departure', 'arrival', 'earlierThan' or 'laterThan'");

		if (query.departure && query.arrival) throw new HttpError(400, "Either departure or arrival can be set, not both");
		if (query.earlierThan && query.laterThan)
			throw new HttpError(400, "Either earlierThan or laterThan can be set, not both");

		if (query.departure && !DateTime.fromISO(query.departure).isValid) throw new HttpError(400, "Invalid departure date");
		if (query.arrival && !DateTime.fromISO(query.arrival).isValid) throw new HttpError(400, "Invalid arrival date");

		return await fetchRoute(query);
	}
}

const fetchRoute = async (query: RoutePlannerQuery): Promise<RouteData> => {
	const params = new URLSearchParams({
		from: query.from.toString(),
		to: query.to.toString(),
		stopovers: "true"
	});
	if ((query?.disabledProducts?.length ?? 0) > 0) disallowProducts(query?.disabledProducts!)
		.forEach((product) => params.append(Object.keys(product)[0], "false"));

	if (query.earlierThan) params.set("earlierThan", query.earlierThan);
	else if (query.laterThan) params.set("laterThan", query.laterThan);
	else {
		params.set("results", query.results!.toString());

		if (query.departure) params.set("departure", query.departure);
		else if (query.arrival) params.set("arrival", query.arrival);
	}

	const request = await fetch(`https://vendo-prof-db.voldechse.wtf/journeys?${params.toString()}`, { method: "GET" });
	if (!request.ok) throw new Error("Failed to fetch route");

	return mapToRoute(await request.json()) as RouteData;
};

const disallowProducts = (input: string[]): { [product: string]: boolean }[] => {
	return input.filter((product) => Object.values(Products).some((p) => p.value === product))
		.map((product) => {
			const object = Object.values(Products).find((p) => p.value === product);
			return { [object?.possibilities?.slice(-1)[0] ?? "ficken"]: false };
		});
};
