// @ts-types="npm:@types/express"
import express from "npm:express";
import { BahnhofHandler } from "./bahnhofHandler.ts";
import { VendoHandler } from "./vendoHandler.ts";
import { CombinedHandler } from "./combinedHandler.ts";

export class TimetableController {
	public router = express.Router();

	private bahnhofHandler = new BahnhofHandler();
	private vendoHandler = new VendoHandler();
	private combinedHandler = new CombinedHandler();

	constructor() {
		this.router.get("/api/v1/timetable", this.combinedHandler.handleRequest.bind(this.combinedHandler));
		this.router.get("/api/v1/timetable/bahnhof", this.bahnhofHandler.handleRequest.bind(this.bahnhofHandler));
		this.router.get("/api/v1/timetable/vendo", this.vendoHandler.handleSingle.bind(this.vendoHandler));
	}
}
