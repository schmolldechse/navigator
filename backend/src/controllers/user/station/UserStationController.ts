import { Controller, Example, Get, Path, Post, Request, Route, Security, Tags } from "tsoa";
import express from "express";
import { auth } from "../../../lib/auth/auth.ts";
import { db } from "../../../lib/db/postgres-data-db.ts";
import { favoriteStations } from "../../../db/data.schema.ts";
import { and, eq } from "drizzle-orm";

class FavoredResponse {
	@Example(8000096)
	evaNumber!: number;

	@Example(true)
	favored!: boolean;
}

@Route("user/station")
@Tags("User")
export class UserStationController extends Controller {

	@Get("favored")
	@Security("better_auth")
	async favored(
		@Request() req: express.Request
	): Promise<FavoredResponse[]> {
		const session = await auth.api.getSession({ headers: new Headers(req.headers as Record<string, string>) });

		const favors = await db
			.select()
			.from(favoriteStations)
			.where(eq(favoriteStations.userId, session?.user?.id!));
		return favors.flatMap((favor) => ({
			evaNumber: favor.evaNumber,
			favored: true
		})) as FavoredResponse[];
	}

	@Get("favored/{evaNumber}")
	@Security("better_auth")
	async isFavored(
		@Path() evaNumber: number,
		@Request() req: express.Request
	): Promise<FavoredResponse> {
		const session = await auth.api.getSession({ headers: new Headers(req.headers as Record<string, string>) });

		const [result] = await db
			.select()
			.from(favoriteStations)
			.where(and(eq(favoriteStations.userId, session?.user?.id!), eq(favoriteStations.evaNumber, evaNumber)))
			.limit(1);
		return { evaNumber: evaNumber, favored: !!result };
	}

	@Post("favor/{evaNumber}")
	@Security("better_auth")
	async favor(
		@Path() evaNumber: number,
		@Request() req: express.Request
	): Promise<FavoredResponse> {
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

		// TODO: check if station exists
		await db.insert(favoriteStations).values({
			userId: session?.user?.id!,
			evaNumber: evaNumber
		});
		return { evaNumber: evaNumber, favored: true };
	}
}
