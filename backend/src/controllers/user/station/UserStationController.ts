import { Controller, Get, Route, Security, Tags, Request } from "tsoa";
import express from "express";
import { auth } from "../../../lib/auth/auth.ts";

@Route("user/station")
@Tags("")
export class UserStationController extends Controller {
	@Get("test")
	@Security("better_auth")
	async favorStation(
		@Request() req: express.Request,
	): Promise<any> {
		const session = await auth.api.getSession({ headers: new Headers(req.headers as Record<string, string>) });
		console.log(session?.user);
		return;
	}
}