import Elysia, { t } from "elysia";
import { HttpStatus } from "../response/status";
import { auth } from "../auth/auth";
import { HttpError } from "../response/error";
import { StationSchema } from "../models/elysia/station.model";
import { UserService } from "../services/user.service";

const userService = new UserService();

const userController = new Elysia({ prefix: "/user", tags: ["User"] })
	.decorate({ httpStatus: HttpStatus })
	.derive(async ({ headers }) => {
		const session = await auth.api.getSession({ headers: new Headers(headers as Record<string, string>) });
		if (!session) throw new HttpError(HttpStatus.HTTP_401_UNAUTHORIZED, "Unauthorized");

		return {
			user: session.user,
			session: session.session
		};
	})
	.group("/favored", (app) =>
		app
			.get(
				"/",
				async ({ user }) => userService.getFavoredStationsBy(user.id),
				{
					detail: {
						summary: "Get favored stations",
						description: "Returns a list of stations that the user has marked as favorite."
					},
					response: t.Array(StationSchema)
				}
			)
			.get("/:evaNumber", async ({ user, params: { evaNumber } }) => userService.isStationFavoredBy(user.id, evaNumber), {
				detail: {
					summary: "Check if a station is favored",
					description: "Checks if the specified station is marked as favorite by the user."
				},
				response: userService.StationFavoredResponse,
				params: t.Object({
					evaNumber: t.Number({
						description: "The evaNumber of a station to check if it is favored."
					})
				})
			})
			.post("/:evaNumber", async ({ user, params: { evaNumber } }) => userService.favorStationBy(user.id, evaNumber), {
				detail: {
					summary: "Toggle favor for a station",
					description: "Toggles the favorite status of the specified station for the user. If the station is already favored, it will be unfavored, and vice versa."
				},
				response: userService.StationFavoredResponse,
				params: t.Object({
					evaNumber: t.Number({
						description: "The evaNumber of the station to toggle favor."
					})
				})
			})
	);

export default userController;
