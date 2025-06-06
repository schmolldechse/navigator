import { t } from "elysia";
import { SmallStopSchema } from "./station.model";
import { MessageSchema } from "./message.model";
import { TimeSchema } from "./time.model";

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
	origin: SmallStopSchema,
	provenance: t.Optional(t.String({ description: "The provenance station name. Mainly used wit HAFAS." })),
	destination: SmallStopSchema,
	direction: t.Optional(t.String({ description: "The direction's station name. Mainly used with HAFAS." })),
	viaStops: t.Optional(t.Array(SmallStopSchema, { description: "List of intermediate stops of the journey" })),
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

export { SingleTimetableEntrySchema, GroupedTimetableEntrySchema };