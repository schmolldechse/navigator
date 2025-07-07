import { StationSchema } from "../models/elysia/station.model";
import { database } from "../db/postgres";
import { favoriteStations } from "../db/user.schema";
import { and, eq, inArray, sql } from "drizzle-orm";
import { stationProducts, stationRil, stations } from "../db/core.schema";
import { t } from "elysia";
import { HttpError } from "../response/error";
import { HttpStatus } from "../response/status";

class UserService {
	readonly StationFavoredResponse = t.Object({
		evaNumber: t.Number(),
		favored: t.Boolean()
	});

	async getFavoredStationsBy(userId: string): Promise<(typeof StationSchema.static)[]> {
		let evaNumberFavors = await database
			.select({ evaNumber: favoriteStations.evaNumber })
			.from(favoriteStations)
			.where(eq(favoriteStations.userId, userId));
		if (evaNumberFavors.length === 0) return [];

		const evaNumbers = evaNumberFavors.map((favor) => favor.evaNumber);

		const stationFavors = await database
			.select({
				evaNumber: stations.evaNumber,
				name: stations.name,
				latitude: stations.latitude,
				longitude: stations.longitude,
				products: sql<
					string[]
				>`COALESCE(array_agg(DISTINCT ${stationProducts.name}) FILTER (WHERE ${stationProducts.name} IS NOT NULL), ARRAY[]::text[])`,
				ril100: sql<
					string[]
				>`COALESCE(array_agg(DISTINCT ${stationRil.ril100}) FILTER (WHERE ${stationRil.ril100} IS NOT NULL), ARRAY[]::text[])`
			})
			.from(stations)
			.leftJoin(stationProducts, eq(stations.evaNumber, stationProducts.evaNumber))
			.leftJoin(stationRil, eq(stations.evaNumber, stationRil.evaNumber))
			.where(inArray(stations.evaNumber, evaNumbers))
			.groupBy(stations.evaNumber, stations.name, stations.latitude, stations.longitude);

		return stationFavors.map((station) => ({
			evaNumber: station.evaNumber,
			name: station.name,
			coordinates: {
				latitude: station.latitude,
				longitude: station.longitude
			},
			products: station.products,
			ril100: station.ril100
		}));
	}

	async isStationFavoredBy(userId: string, evaNumber: number): Promise<typeof this.StationFavoredResponse.static> {
		const [existing] = await database
			.select()
			.from(favoriteStations)
			.where(and(eq(favoriteStations.userId, userId), eq(favoriteStations.evaNumber, evaNumber)))
			.limit(1);
		return { evaNumber, favored: !!existing };
	}

	async favorStationBy(userId: string, evaNumber: number): Promise<typeof this.StationFavoredResponse.static> {
		const [existing] = await database
			.select()
			.from(favoriteStations)
			.where(and(eq(favoriteStations.userId, userId), eq(favoriteStations.evaNumber, evaNumber)))
			.limit(1);
		if (existing) {
			await database
				.delete(favoriteStations)
				.where(and(eq(favoriteStations.userId, userId), eq(favoriteStations.evaNumber, evaNumber)));
			return { evaNumber: evaNumber, favored: false };
		}

		// check if the station exists
		const stationExists = await database.select().from(stations).where(eq(stations.evaNumber, evaNumber)).limit(1);
		if (stationExists.length === 0) throw new HttpError(HttpStatus.HTTP_404_NOT_FOUND, "Station does not exist");

		// favor the station
		await database.insert(favoriteStations).values({
			userId: userId,
			evaNumber: evaNumber
		});
		return { evaNumber: evaNumber, favored: true };
	}
}

export { UserService };
