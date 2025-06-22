import Elysia from "elysia";
import { HttpStatus } from "../response/status";
import { HttpError } from "../response/error";
import { DateTime } from "luxon";
import { StatisticsService } from "../services/statistics.service";
import { JourneyStatisticsSchema } from "../models/elysia/analytics.model";
import { stationRil } from "../db/core.schema";
import { inArray } from "drizzle-orm";
import { database } from "../db/postgres";

const statisticsService = new StatisticsService();

const statisticsController = new Elysia({ prefix: "/statistics", tags: ["Statistics"] })
	.decorate({ httpStatus: HttpStatus })
	.post(
		"/",
		async ({ body }) => {
			body.startDate = (body.startDate || statisticsService.getLastTimetableChange()).startOf("day");
			body.endDate = (body.endDate || DateTime.now()).endOf("day");
			if (body.startDate > body.endDate) throw new HttpError(400, "Start date can't be after end date");

			if (body.options.enableRilLookup) {
				let ril100 = (
					await database
						.select({ ril100: stationRil.ril100 })
						.from(stationRil)
						.where(inArray(stationRil.evaNumber, body.evaNumbers))
				).map((row) => row.ril100);
				ril100 = Array.from(new Set(ril100)); // remove duplicates

				let evaNumbers = (
					await database
						.select({ evaNumber: stationRil.evaNumber })
						.from(stationRil)
						.where(inArray(stationRil.ril100, ril100))
				).map((row) => row.evaNumber);

				body.evaNumbers = Array.from(new Set([...body.evaNumbers, ...evaNumbers])); // merge and remove duplicates
			}

			return await statisticsService.startEvaluation(body);
		},
		{
			detail: {
				summary: "Get statistics for a station",
				description: "Get statistics for a station by its evaNumber."
			},
			body: statisticsService.statsBody,
			response: JourneyStatisticsSchema
		}
	);

export default statisticsController;
