import { type Station } from "navigator-core/src/models/station";
import { HttpError } from "../response/error";
import { HttpStatus } from "../response/status";
import { v4 as uuid } from "uuid";
import { database } from "../db/postgres";
import { stationProducts, stationRil, stations } from "../db/core.schema";
import { eq } from "drizzle-orm";

class StationService {
	fetchStations = async (searchTerm: string): Promise<Station[]> => {
		const request = await fetch("https://app.vendo.noncd.db.de/mob/location/search", {
			method: "POST",
			headers: {
				Accept: "application/x.db.vendo.mob.location.v3+json",
				"Content-Type": "application/x.db.vendo.mob.location.v3+json",
				"X-Correlation-ID": uuid() + "_" + uuid()
			},
			body: JSON.stringify({
				searchTerm,
				maxResults: 10,
				locationTypes: ["ALL"]
			})
		});
		if (!request.ok)
			throw new HttpError(HttpStatus.HTTP_502_BAD_GATEWAY, `Could not find any stations related to ${searchTerm}`);

		const response = await request.json();
		if (!response || !Array.isArray(response))
			throw new HttpError(
				HttpStatus.HTTP_502_BAD_GATEWAY,
				`Response was expected to be an array, but got ${typeof response}`
			);

		const stationList: Station[] = response
			.filter((stationElement: any) => /^\d+$/.test(stationElement?.evaNr))
			.map(
				(stationElement: any) =>
					({
						name: stationElement.name,
						evaNumber: Number(stationElement.evaNr),
						coordinates: {
							latitude: stationElement.coordinates.latitude,
							longitude: stationElement.coordinates.longitude
						},
						products: stationElement.products ?? []
					}) as Station
			);
		return await Promise.all(
			stationList.map((station) => this.saveStation(station))
		);
	};

	private saveStation = async (station: Station): Promise<Station> => {
		const existing = await database.select().from(stations).where(eq(stations.evaNumber, station.evaNumber)).limit(1);
		if (existing.length === 0)
			// insert station object itself
			await database.insert(stations).values({
				evaNumber: station.evaNumber,
				name: station.name,
				latitude: station.coordinates.latitude,
				longitude: station.coordinates.longitude
			});

		// insert products
		const existingProducts = (
			await database.select().from(stationProducts).where(eq(stationProducts.evaNumber, station.evaNumber))
		).map((product) => product.name.toLowerCase());
		const newProducts = station.products.filter((product: string) => !existingProducts.includes(product.toLowerCase()));
		if (newProducts.length > 0) {
			const promises = newProducts.map((product: string) =>
				database.insert(stationProducts).values({
					evaNumber: station.evaNumber,
					name: product,
					queryingEnabled: false
				})
			);
			await Promise.all(promises);
		}

		// update ril100
		const ril100 = (await database.select().from(stationRil).where(eq(stationRil.evaNumber, station.evaNumber))).map(
			(ril) => ril.ril100
		);
		if (ril100.length > 0) station.ril100 = ril100;
		return station;
	};

	getCachedStation = async (evaNumber: number): Promise<StationDocument | null> => {
		const collection = await getCollection("stations");
		return (await collection.findOne({ evaNumber })) as StationDocument;
	};

	/**
	 * Since a station can have multiple ril100 identifiers, we try to gather all `evaNumber`'s from a station
	 * This may happen at larger stations, for example:
	 * - Stuttgart Hbf
	 * - Hauptbf (Arnulf-Klett-Platz), Stuttgart
	 */
	getRelatedEvaNumbers = async (station: Station): Promise<number[]> => {
		if ((station?.ril100 || []).length > 0) {
			const collection = await getCollection("stations");
			return (await collection.find({ ril100: { $in: station.ril100 } }).toArray()).map(
				(station: Station) => station?.evaNumber
			);
		}
		return [station?.evaNumber];
	};
}

export { StationService };
