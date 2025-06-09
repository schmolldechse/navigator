import { t } from "elysia";
import { BasicStationSchema } from "./station.model";
import { MessageSchema } from "./message.model";
import { TimeSchema } from "./time.model";

const TimetableStopSchema = t.Intersect([
	BasicStationSchema,
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

const SingleTimetableEntrySchema = t.Object({
	ris_journeyId: t.Optional(t.String({
		description: "Identifier for the ReisendenInformationsSystem (RIS) from the Deutsche Bahn. The first 8 characters (in `yyyyMMdd` format) of the Id represents the date when the journey takes place.",
		error: validation => {
			console.log(validation.errors);
		}
	})),
	hafas_journeyId: t.Optional(t.String({
		description: "Identifier for the journey in the HAFAS system."
	})),
	origin: TimetableStopSchema,
	destination: TimetableStopSchema,
	viaStops: t.Optional(t.Array(TimetableStopSchema, { description: "List of intermediate stops of the journey" })),
	timeInformation: TimeSchema,
	lineInformation: t.Object({
		productType: t.String({ description: "The product type of the journey", examples: ["NahverkehrsonstigeZuege"] }),
		productName: t.String({ description: "The product category of the journey", examples: ["RB"] }),
		journeyNumber: t.Optional(t.Number({ description: "The unique id of the journey per day", examples: [63] })),
		journeyName: t.String({ description: "The name of this journey", examples: ["RB 63"] }),
		additionalJourneyName: t.Optional(t.String({
			description: "Additional name for this journey",
			examples: ["RE 87"]
		})),
		operator: t.Optional(t.Object({
			code: t.Optional(t.String({ description: "The operator's code" })),
			name: t.Optional(t.String({ description: "The name of the operator", examples: ["DB Fernverkehr AG"] }))
		}))
	}),
	messages: t.Array(MessageSchema),
	cancelled: t.Boolean({ description: "Indicates if the journey is cancelled." })
});

const GroupedTimetableEntrySchema = t.Object({
	entries: t.Array(SingleTimetableEntrySchema)
});

export { TimetableStopSchema, SingleTimetableEntrySchema, GroupedTimetableEntrySchema };