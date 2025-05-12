import { type Station } from "navigator-core/src/models/station";
import { mapToProduct, Products } from "navigator-core/src/models/products";
import { HttpError } from "../response/error";
import { HttpStatus } from "../response/status";
import { getCollection, StationDocument } from "../db/mongodb/mongodb";

class StationService {
	fetchStations = async (searchTerm: string): Promise<Station[]> => {
		const params = new URLSearchParams({
			query: searchTerm,
			limit: "10"
		});
		const request = await fetch(`https://vendo-prof-db.voldechse.wtf/locations?${params.toString()}`, { method: "GET" });
		if (!request.ok)
			throw new HttpError(HttpStatus.HTTP_502_BAD_GATEWAY, `Could not find any stations related to ${searchTerm}`);

		const response = await request.json();
		if (!response || !Array.isArray(response))
			throw new HttpError(
				HttpStatus.HTTP_502_BAD_GATEWAY,
				`Response was expected to be an array, but got ${typeof response}`
			);

		return response
			.filter((data: any) => /^\d+$/.test(data?.id))
			.map(
				(data: any) =>
					({
						name: data?.name,
						evaNumber: Number(data?.id),
						coordinates: {
							latitude: data?.location?.latitude,
							longitude: data?.location?.longitude
						},
						// either "ril100Ids" is included directly in the object, or it is, for whatever reason, contained in a nested "station" object
						ril100: (data?.ril100Ids || data?.station?.ril100Ids || []).map((ril100Id: string) => ril100Id),
						products: Object.entries(data?.products || [])
							.filter(([key, value]) => value === true)
							.map(([key]) => mapToProduct(key))
							.filter((product) => product.value !== Products.UNKNOWN.value)
							.map((product) => product.value)
					}) as Station
			) as Station[];
	};

	fetchAndCacheStations = async (searchTerm: string): Promise<Station[]> => {
		const stations: Station[] = await this.fetchStations(searchTerm);
		if (stations.length > 0) await this.cacheStations(stations);

		return stations;
	}

	private cacheStations = async (stations: Station[]): Promise<void> => {
		const collection = await getCollection("stations");

		const bulkOps = stations.map((station) => ({
			updateOne: {
				filter: { evaNumber: station.evaNumber },
				update: {
					$set: {
						name: station.name,
						coordinates: station.coordinates,
						ril100: station.ril100,
						products: station.products
					}
				},
				upsert: true
			}
		}));

		await collection.bulkWrite(bulkOps);
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
