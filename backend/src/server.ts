import Elysia from "elysia";
import swagger from "@elysiajs/swagger";
import { authApp } from "./auth/auth";
import stationController from "./controllers/station.controller";
import { HttpStatus } from "./response/status";
import { HttpError } from "./response/error";

const restApi = new Elysia({ prefix: "/api" })
	.error({ HttpError })
	.onError({ as: 'global' }, ({ error, set, code }) => {
		let message = "Internal Server Error";
		set.status = HttpStatus.HTTP_500_INTERNAL_SERVER_ERROR;

		switch (code) {
			case "HttpError":
				set.status = error.httpStatus;
				message = error.message;
				break;
		}

		return Response.json({ error: message, status: set.status });
	})
	.use(stationController);

const app = new Elysia()
	.use(swagger({
		path: "/api",
		scalarConfig: {
			hideDarkModeToggle: true,
			// needs to be empty that the theme is changeable: {@link https://github.com/elysiajs/elysia-swagger/issues/194}
			customCss: "",
		},
		documentation: {
			info: {
				title: "Navigator Backend",
				description: "API documentation for the Navigator backend",
				version: "1.0.0"
			}
		}
	}))
	.use(authApp)
	.use(restApi)
	.listen(3000);

console.log(`Elysia server is running at ${app.server?.hostname}:${app.server?.port}`);
