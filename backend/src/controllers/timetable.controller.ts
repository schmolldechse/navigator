import Elysia, { t } from "elysia";
import { HttpStatus } from "../response/status";
import { RequestType, Profile, TimetableService } from "../services/timetable.service";

const timetableService = new TimetableService();

const timetableController = new Elysia({ prefix: "/timetable", tags: ["Timetable"] }).decorate({ httpStatus: HttpStatus }).get(
	"/:evaNumber/:type",
	({ params: { evaNumber, type }, query }) => {
		return timetableService.retrieveConnections(evaNumber, type, Profile.RIS, query);
	},
	{
		params: t.Object({
			evaNumber: t.Number({
				description: "evaNumber to look for",
				error: "Parameter 'evaNumber' is required and must be a number."
			}),
			type: t.Enum(RequestType, {
				default: RequestType.DEPARTURES,
				description: "Type of timetable to display",
				error: "Parameter 'type' must be either 'departures' or 'arrivals'."
			})
		}),
		query: timetableService.query,
		detail: {
			summary: "List timetable for a station",
			description: "Get the timetable for a station by its evaNumber."
		}
	}
);
export default timetableController;
