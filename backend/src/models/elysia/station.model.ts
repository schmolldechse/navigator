import { t } from "elysia";
import { TimeSchema } from "./time.model";
import { MessageSchema } from "./message.model";

const BasicSchema = t.Object({
	name: t.String({ description: "Name of the station" }),
	evaNumber: t.Number({ description: "Unique evaNumber of the station" })
});

const StationSchema = t.Intersect([
	BasicSchema,
	t.Object({
		coordinates: t.Object(
			{
				latitude: t.Number({ description: "Latitude of the station" }),
				longitude: t.Number({ description: "Longitude of the station" })
			},
			{ description: "Coordinates of the station" }
		),
		ril100: t.Optional(t.Array(t.String(), { description: "List of RIL100 numbers of the station" })),
		products: t.Array(t.String(), { description: "List of products available at the station" })
	})
]);

const SmallStopSchema = t.Intersect([
	BasicSchema,
	t.Object({
		cancelled: t.Boolean({ description: "Marks if the stop is cancelled" }),
		additional: t.Optional(t.Boolean({ description: "Marks if the stop is additional" })),
		separation: t.Optional(t.Boolean({ description: "Specifies if there is an separation of vehicles at this station" })),
		nameParts: t.Optional(t.Array(t.Object({
			type: t.String(),
			value: t.String(),
		}), { description: "List of name parts" }))
	})
]);

const LargeStopSchema = t.Intersect([
	SmallStopSchema,
	t.Object({
		departure: t.Optional(TimeSchema),
		arrival: t.Optional(TimeSchema),
		messages: t.Array(MessageSchema)
	})
]);

export { StationSchema, SmallStopSchema, LargeStopSchema };