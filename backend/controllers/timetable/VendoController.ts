import { DateTime } from "luxon";
import { Profile, RequestType, retrieveCombinedConnections, retrieveConnections } from "./requests.ts";
import { Controller, Get, Queries, Res, Route, type TsoaResponse } from "tsoa";
import type { Journey } from "../../models/connection.ts";

class VendoQuery {
	evaNumber!: string;
	type!: RequestType;
	profile!: Profile;
	when?: string;
	duration?: number = 60;
	results?: number = 1000;
}

@Route("timetable/vendo")
export class VendoController extends Controller {
	@Get()
	async getVendoJourneys(
		@Queries() query: VendoQuery,
		@Res() badRequestResponse: TsoaResponse<400, { reason: string }>
	): Promise<Journey[]> {
		query = {
			...query,
			when: query.when ?? DateTime.now().set({ second: 0, millisecond: 0 }).toISO()
		};

		if (!DateTime.fromISO(query.when!).isValid) {
			return badRequestResponse(400, { reason: "Invalid date" });
		}

		if (query.profile === Profile.COMBINED) {
			return (await retrieveCombinedConnections(query)).map((connection) => ({
				connections: [connection]
			}));
		}

		return (await retrieveConnections(query)).map((connection) => ({
			connections: [connection]
		}));
	}
}
