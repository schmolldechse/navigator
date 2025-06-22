import Elysia from "elysia";
import swagger from "@elysiajs/swagger";
import { authApp } from "./auth/auth";
import { HttpStatus } from "./response/status";
import { HttpError } from "./response/error";
import stationController from "./controllers/station.controller";
import timetableController from "./controllers/timetable.controller";
import routeController from "./controllers/route.controller";
import statisticsController from "./controllers/statistics.controller";

const restApi = new Elysia({ prefix: "/api" })
	.error({ HttpError })
	.onError({ as: "global" }, ({ error, set, code }) => {
		let message = "Internal Server Error";
		set.status = HttpStatus.HTTP_500_INTERNAL_SERVER_ERROR;

		switch (code) {
			case "HttpError":
				set.status = error.httpStatus;
				message = error.message;
				break;
			case "VALIDATION":
				set.status = error.status ?? HttpStatus.HTTP_422_UNPROCESSABLE_ENTITY;
				message = error.validator ? (error.validator.Errors(error.value).First().schema?.error || error.validator.Errors(error.value).First().message) : error.message;
				break;
			case "NOT_FOUND":
				set.status = error.status;
				message = "Route does not exist";
				break;
			case "UNKNOWN":
				set.status = HttpStatus.HTTP_500_INTERNAL_SERVER_ERROR;
				message = "The server encountered an unexpected condition and could not fulfill the request.";

				console.error("Unknown error occurred:", error);
				break;
		}

		return Response.json({ error: message, status: set.status });
	})
	.use(stationController)
	.use(timetableController)
	.use(routeController)
	.use(statisticsController);

const app = new Elysia()
	.use(
		swagger({
			path: "/api",
			scalarConfig: {
				hideDarkModeToggle: true,
				// needs to be empty that the theme is changeable: {@link https://github.com/elysiajs/elysia-swagger/issues/194}
				customCss: ""
			},
			documentation: {
				info: {
					title: "Navigator Backend",
					description: "API documentation for the Navigator backend",
					version: "1.0.0"
				}
			}
		})
	)
	.use(authApp)
	.use(restApi)
	.listen(3000);

console.log(`Elysia server is running at ${app.server?.hostname}:${app.server?.port}`);
