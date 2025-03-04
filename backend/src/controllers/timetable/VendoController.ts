import { DateTime } from "luxon";
import { Profile, RequestType, retrieveCombinedConnections, retrieveConnections } from "./requests.ts";
import { Controller, Get, Queries, Res, Route, Tags, type TsoaResponse } from "tsoa";
import type { Journey } from "../../models/connection.ts";

class VendoQuery {
	evaNumber!: number;
	type!: RequestType;
	profile!: Profile;
	when?: string;
	duration?: number = 60;
	results?: number = 1000;
}

@Route("timetable/vendo")
@Tags("Vendo")
export class VendoController extends Controller {
	@Get()
	async getVendoJourneys(
		@Queries() query: VendoQuery
	): Promise<Journey[]> {
		query = {
			...query,
			when: query.when ?? DateTime.now().set({ second: 0, millisecond: 0 }).toISO()
		};

		if (!DateTime.fromISO(query.when!).isValid) {
			throw new HttpError(400, "Invalid date");
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
