import { Controller, Get, Route, Tags } from "tsoa";
import { fromLineName } from "../../lib/converter.ts";
import type { LineColor } from "../../models/connection.ts";

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
}
