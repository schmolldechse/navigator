import Elysia, { t } from "elysia";
import { HttpStatus } from "../response/status";
import { RouteService } from "../services/route.service";
import { RouteDetailsSchema } from "../models/elysia/route.model";

const routeService = new RouteService();

const routeController = new Elysia({ prefix: "/route", tags: ["Routes"] })
	.decorate({ httpStatus: HttpStatus })
	.post("/", async ({ body }) => await routeService.retrieveRoutes(body), {
		detail: {
			summary: "Get routes",
			description: "Fetches routes based on the two provided stations."
		},
		body: routeService.routeBody,
		response: RouteDetailsSchema
	});

export default routeController;
