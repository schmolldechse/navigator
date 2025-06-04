import { t } from "elysia";

const TimeSchema = t.Object(
	{
		plannedTime: t.String({ description: "Planned time at the stop", format: "date-time" }),
		actualTime: t.String({ description: "Actual time at the stop", format: "date-time" }),
		delay: t.Optional(t.Number({ description: "Delay in seconds" })),
		plannedPlatform: t.Optional(t.String({ description: "Planned platform at the stop" })),
		actualPlatform: t.Optional(t.String({ description: "Actual platform at the stop" }))
	},
	{
		description: "Object representing a time at a stop"
	}
);

export { TimeSchema };