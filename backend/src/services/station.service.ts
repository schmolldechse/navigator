import { Station } from "../models/core/models";
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
		return await Promise.all(stationList.map((station) => this.saveStation(station)));
	};

	fetchStationByEvaNumber = async (evaNumber: number): Promise<Station> => {
		const request = await fetch(`https://app.vendo.noncd.db.de/mob/location/details/${evaNumber}`, {
			method: "GET",
			headers: {
				Accept: "application/x.db.vendo.mob.location.v3+json",
				"Content-Type": "application/x.db.vendo.mob.location.v3+json",
				"X-Correlation-ID": uuid() + "_" + uuid()
			}
		});
		if (!request.ok)
			throw new HttpError(HttpStatus.HTTP_502_BAD_GATEWAY, `Could not find any stations related to ${evaNumber}`);

		const response = await request.json();
		if (!response)
			throw new HttpError(
				HttpStatus.HTTP_502_BAD_GATEWAY,
				`Response was expected to be an object, but got ${typeof response}`
			);

		if (!response.haltName)
			throw new HttpError(
				HttpStatus.HTTP_502_BAD_GATEWAY,
				`Response did not match expected format. Missing 'haltName' property.`
			);

		const station: Station = {
			name: response.haltName,
			products: (response.produktGattungen ?? []).map((productElement: any) => productElement.produktGattung),
			evaNumber: -1,
			coordinates: {
				latitude: -1,
				longitude: -1
			}
		};

		// search for a station by name
		const existingStation = await database.select().from(stations).where(eq(stations.name, station.name)).limit(1);
		if (existingStation.length === 0)
			throw new HttpError(HttpStatus.HTTP_502_BAD_GATEWAY, `Could not find any station with ${station.name}`);

		// update values
		station.evaNumber = Number(existingStation[0].evaNumber);
		station.coordinates = {
			latitude: existingStation[0].latitude,
			longitude: existingStation[0].longitude
		};
		return this.saveStation(station, false);
	};

	getRelatedEvaNumbers = async (evaNumber: number): Promise<number[]> => {
		const rilMap = (
			await database.select({ ril100: stationRil.ril100 }).from(stationRil).where(eq(stationRil.evaNumber, evaNumber))
		).flatMap((ril) => ril.ril100);
		if (rilMap.length === 0) return [evaNumber];

		const evaNumbers = (
			await Promise.all(
				rilMap.map(async (ril100) =>
					(
						await database
							.select({ evaNumber: stationRil.evaNumber })
							.from(stationRil)
							.where(eq(stationRil.ril100, ril100))
					).flatMap((ril) => ril.evaNumber)
				)
			)
		).flat();

		return [...new Set(evaNumbers)];
	};

	private saveStation = async (station: Station, insertNew: boolean = true): Promise<Station> => {
		if (insertNew) {
			const existing = await database.select().from(stations).where(eq(stations.evaNumber, station.evaNumber)).limit(1);
			if (existing.length === 0)
				// insert station object itself
				await database.insert(stations).values({
					evaNumber: station.evaNumber,
					name: station.name,
					latitude: station.coordinates.latitude,
					longitude: station.coordinates.longitude
				});
		}

		// insert products
		const existingProducts = (
			await database.select().from(stationProducts).where(eq(stationProducts.evaNumber, station.evaNumber))
		).map((product) => product.name.toLowerCase());
		const newProducts = station.products.filter((product: string) => !existingProducts.includes(product.toLowerCase()));
		if (newProducts.length > 0) {
			await database.insert(stationProducts).values(
				newProducts.map((product: string) => ({
					evaNumber: station.evaNumber,
					name: product,
					queryingEnabled: false
				}))
			);
		}

		// update ril100
		const ril100 = (await database.select().from(stationRil).where(eq(stationRil.evaNumber, station.evaNumber))).map(
			(ril) => ril.ril100
		);
		if (ril100.length > 0) station.ril100 = ril100;
		return station;
	};
}

export { StationService };
