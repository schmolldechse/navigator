import Elysia, { t } from "elysia";
import { HttpStatus } from "../response/status";
import { RouteService } from "../services/route.service";
import { RouteDetailsSchema } from "../models/elysia/route.model";

const routeService = new RouteService();

const routeController = new Elysia({ prefix: "/route", tags: ["Routes"] })
	.post("/", async ({ body }) => await routeService.retrieveRoutes(body), {
		detail: {
			summary: "Get route details",
			description: "Fetches routes based on the two provided stations.",
		},
		body: routeService.routeBody,
		response: RouteDetailsSchema
	})
	.decorate({ httpStatus: HttpStatus });

export default routeController;