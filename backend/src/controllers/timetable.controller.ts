import Elysia, { t } from "elysia";
import { HttpStatus } from "../response/status";
import { Profile, RequestType, TimetableService } from "../services/timetable.service";
import { HttpError } from "../response/error";
import { GroupedTimetableEntrySchema } from "../models/elysia/timetable.model";

const timetableService = new TimetableService();

const params = t.Object({
	evaNumber: t.Number({
		description: "evaNumber to look for",
		error: "Parameter 'evaNumber' is required and must be a number."
	}),
	type: t.Enum(RequestType, {
		default: RequestType.DEPARTURES,
		description: "Type of timetable to display",
		error: "Parameter 'type' must be either 'departures' or 'arrivals'."
	})
});

const timetableController = new Elysia({
	prefix: "/timetable",
	tags: ["Timetable"]
})
	.decorate({ httpStatus: HttpStatus })
	.get("/:evaNumber/:type", ({ params: { evaNumber, type }, query }) => {}, {
		params,
		query: timetableService.query,
		detail: {
			summary: "List timetable for a station",
			description: "Get the timetable for a station by its evaNumber."
		}
	})
	.get(
		"/profile/:profile/:evaNumber/:type",
		({ params: { profile, evaNumber, type }, query }) => {
			switch (profile) {
				case Profile.BAHNHOF:
					return timetableService.retrieveBahnhofConnections(evaNumber, type, query);
				case Profile.RIS:
					return timetableService.retrieveRISConnections(evaNumber, type, query);
				case Profile.HAFAS:
					return timetableService.retrieveHAFASConnections(evaNumber, type, query);
				default:
					throw new HttpError(HttpStatus.HTTP_400_BAD_REQUEST, "Invalid profile");
			}
		},
		{
			params: t.Intersect([
				params,
				t.Object({
					profile: t.Enum(Profile, {
						default: Profile.RIS,
						description: "Profile to use for the request",
						error: "Parameter 'profile' must be either 'bahnhof', 'ris' or 'hafas'."
					})
				})
			]),
			query: timetableService.query,
			detail: {
				summary: "Profile specific timetable",
				description:
					"Get the timetable for a station by its evaNumber and profile.\n\nKeep in mind, that the 'when' parameter is ignored for the 'bahnhof' profile."
			},
			response: t.Array(GroupedTimetableEntrySchema)
		}
	);
export default timetableController;
