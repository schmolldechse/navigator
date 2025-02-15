import { Controller, Get, Queries, Res, Route, Tags, type TsoaResponse } from "tsoa";
import { DateTime } from "luxon";
import { mapCoachSequence } from "../../lib/mapping.ts";

class SequenceQuery {
	lineDetails!: string;
	evaNumber!: string;
	date!: string;
}

@Route("journey/sequence")
@Tags("Coach Sequences")
export class SequenceController extends Controller {
	@Get()
	async getSequenceById(
		@Queries() query: SequenceQuery,
		@Res() badRequestResponse: TsoaResponse<400, { reason: string }>
	): Promise<any> {
		if (query.lineDetails === "") {
			return badRequestResponse(400, { reason: "lineDetails must not be empty" });
		}
		if (!/^\d+$/.test(query.evaNumber)) {
			return badRequestResponse(400, { reason: "evaNumber is not an integer" });
		}

		const dateValidation = DateTime.fromFormat(query.date, "yyyyMMdd");
		if (!dateValidation.isValid) {
			return badRequestResponse(400, { reason: `${dateValidation.invalidExplanation}` });
		}

		return fetchCoachSequence(query);
	}
}

const fetchCoachSequence = async (query: SequenceQuery): Promise<any> => {
	const request = await fetch(`https://app.vendo.noncd.db.de/mob/zuglaeufe/${query.lineDetails}/halte/by-abfahrt/${query.evaNumber}_${query.date}/wagenreihung`, {
		method: "GET",
		headers: {
			Accept: "application/x.db.vendo.mob.wagenreihung.v3+json",
			"Content-Type": "application/x.db.vendo.mob.wagenreihung.v3+json",
			"X-Correlation-ID": crypto.randomUUID() + "_" + crypto.randomUUID()
		}
	});

	if (!request.ok) {
		throw new Error("Failed to fetch coach sequence");
	}

	return mapCoachSequence(await request.json());
};