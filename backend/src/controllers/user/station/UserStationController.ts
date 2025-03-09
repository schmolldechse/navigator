import { Controller, Example, Get, Path, Post, Request, Route, Security, Tags } from "tsoa";
import express from "express";
import { auth } from "../../../lib/auth/auth.ts";
import { db } from "../../../lib/db/postgres-data-db.ts";
import { favoriteStations } from "../../../db/postgres/data.schema.ts";
import { and, eq } from "drizzle-orm";
import { connectToDb, getCollection } from "../../../lib/db/mongo-data-db.ts";
import type { Station } from "../../../models/station.ts";
import { HttpError } from "../../../lib/errors/HttpError.ts";

class FavoredResponse {
	@Example(8000096)
	evaNumber!: number;

	@Example(true)
	favored!: boolean;
}

@Route("user/station")
@Tags("User")
@Security("better_auth")
export class UserStationController extends Controller {
	@Get("favored")
	async favored(@Request() req: express.Request): Promise<Station[]> {
		const session = await auth.api.getSession({ headers: new Headers(req.headers as Record<string, string>) });

		const favors = await db.select().from(favoriteStations).where(eq(favoriteStations.userId, session?.user?.id!));
		const evaNumbers = favors.map((favor) => favor.evaNumber);

		const collection = await getCollection("stations");
		return (await collection.find({ evaNumber: { $in: evaNumbers } }).toArray()).map(
			({ _id, lastQueried, queryingEnabled, ...rest }) => rest
		);
	}

	@Get("favored/{evaNumber}")
	async isFavored(@Path() evaNumber: number, @Request() req: express.Request): Promise<FavoredResponse> {
		const session = await auth.api.getSession({ headers: new Headers(req.headers as Record<string, string>) });

		const [result] = await db
			.select()
			.from(favoriteStations)
			.where(and(eq(favoriteStations.userId, session?.user?.id!), eq(favoriteStations.evaNumber, evaNumber)))
			.limit(1);
		return { evaNumber: evaNumber, favored: !!result };
	}

	@Post("favor/{evaNumber}")
	async favor(@Path() evaNumber: number, @Request() req: express.Request): Promise<FavoredResponse> {
		const session = await auth.api.getSession({ headers: new Headers(req.headers as Record<string, string>) });

		const [existing] = await db
			.select()
			.from(favoriteStations)
			.where(and(eq(favoriteStations.userId, session?.user?.id!), eq(favoriteStations.evaNumber, evaNumber)))
			.limit(1);

		if (existing) {
			await db
				.delete(favoriteStations)
				.where(and(eq(favoriteStations.userId, session?.user?.id!), eq(favoriteStations.evaNumber, evaNumber)));
			return { evaNumber: evaNumber, favored: false };
		}

		const station = await checkStation(evaNumber);
		if (!station) throw new HttpError(404, "Station not queried yet");

		await db.insert(favoriteStations).values({
			userId: session?.user?.id!,
			evaNumber: evaNumber
		});
		return { evaNumber: evaNumber, favored: true };
	}
}

const checkStation = async (evaNumber: number): Promise<Station | null> => {
	const client = await connectToDb();
	const collection = client.collection<Station>("stations");

	return (await collection.findOne({ evaNumber })) as Station;
};
