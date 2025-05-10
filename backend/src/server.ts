import Elysia from "elysia";
import swagger from "@elysiajs/swagger";
import { authApp } from "./auth/auth";
import stationController from "./controllers/station.controller";
import { HttpStatus } from "./response/HttpStatus";
import { OpenAPI } from "./auth/auth.controller";

const restApi = new Elysia({ prefix: "/api" })
	.decorate({ httpStatus: HttpStatus })
	.get("/test", () => "Hello World")
	.use(stationController);

const app = new Elysia()
	.use(swagger({
		path: "/api",
		documentation: {
			components: await OpenAPI.components,
			paths: await OpenAPI.getPaths()
		}
	}))
	.use(authApp)
	.use(restApi)
	.listen(3000);

console.log(`Elysia server is running at ${app.server?.hostname}:${app.server?.port}`);
