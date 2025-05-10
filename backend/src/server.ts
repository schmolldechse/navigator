import Elysia from "elysia";
import swagger from "@elysiajs/swagger";
import { authApp } from "./lib/auth/auth";
import stationController from "./controllers/station.controller";
import { HttpStatus } from "./response/HttpStatus";

const restApi = new Elysia({ prefix: "/api" })
	.decorate({ httpStatus: HttpStatus })
	.get("/test", () => "Hello World")
	.use(stationController);

const app = new Elysia()
	.use(swagger({
		path: "/api",
	}))
	.use(authApp)
	.use(restApi)
	.listen(3000);

console.log(`Elysia server is running at ${app.server?.hostname}:${app.server?.port}`);
