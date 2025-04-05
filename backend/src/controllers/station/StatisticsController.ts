import { Controller, Path, Post, Route, Tags } from "tsoa";
import { HttpError } from "../../lib/errors/HttpError.ts";
import { getCollection, type StationDocument } from "../../lib/db/mongo-data-db.ts";
import type { ConnectionDocument } from "../../db/mongodb/station.schema.ts";
import { getCachedStation } from "./request.ts";

@Route("stations")
@Tags("Stations")
export class StatisticsController extends Controller {
	@Post("/stats/{evaNumber}")
	async getStationStatistics(@Path() evaNumber: number): Promise<void> {
		const cachedStation = await getCachedStation(evaNumber);
		if (!cachedStation) throw new HttpError(400, "Station not found");

		/**
		 * since a station can have several evaNumbers, we try to access all evaNumbers using the "ril100" identifier
		 * this can occur at larger stations, for example "Stuttgart Hbf" & "Hauptbf (Arnulf-Klett-Platz), Stuttgart"
		 */
		let evaNumbers: number[] = [];
		if ((cachedStation?.ril100 || []).length > 0) {
			const collection = await getCollection("stations");
			evaNumbers = (await collection.find({ ril100: { $in: cachedStation.ril100 } })
				.toArray())
				.map((station: StationDocument) => station?.evaNumber);
		} else evaNumbers = [cachedStation?.evaNumber];

		const acquireTrips = async (evaNumbers: number[]): Promise<ConnectionDocument[]> => {
			const collection = await getCollection("trips");
			return (await collection.find({ "viaStops.evaNumber": { $in: evaNumbers } }).toArray()) as ConnectionDocument[];
		}

		console.log((await acquireTrips(evaNumbers)).length);
	}
}