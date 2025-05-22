import { t } from "elysia";
import { StopSchema } from "./station";
import { TimeSchema } from "./time";
import { MessageSchema } from "./message";

const datePrefix = new Date().toISOString().slice(0, 10).replace(/-/g, "");

const ConnectionSchema = t.Object({
	ris_journeyId: t.Optional(t.String({
		description: "Unique identifier for DB's ReisendenInformationsSystem (RIS). The first 8 characters of the UUID represent the date of the journey.",
		format: "uuid",
		pattern: "^\\d{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$",
		examples: [
			`${datePrefix}-0000-0000-0000-000000000001`,
			`${datePrefix}-0000-0000-0000-000000000002`
		]
	})),
	hafas_journeyId: t.Optional(t.String({ description: "Unique identifier for the journey in the HAFAS system." })),
	origin: t.Optional(StopSchema),
	provenance: t.Optional(t.String({ description: "The provenance station name, mainly used with HAFAS." })),
	destination: t.Optional(StopSchema),
	actualDestination: t.Optional(StopSchema),
	direction: t.Optional(t.String({ description: "The direction's station name, mainly used with HAFAS." })),
	viaStops: t.Optional(t.Array(StopSchema, { description: "List of intermediate stops of the Connection." })),
	departure: t.Optional(TimeSchema),
	arrival: t.Optional(TimeSchema),
	lineInformation: t.Optional(t.Object({
		type: t.String({ description: "The type of the journey", examples: "NahverkehrsonstigeZuege" }),
		/**
		 * @deprecated
		 */
		replacementServiceType: t.Optional(t.String({
			description: "The type of the replacement service",
			examples: "Busse"
		})),
		product: t.Optional(t.String({ description: "Product category", examples: "IC" })),
		lineName: t.Optional(t.String({ description: "The name of the line", examples: "IC 287" })),
		additionalLineName: t.Optional(t.String({ description: "Additional line name", examples: "RE 87" })),
		fahrtNr: t.Optional(t.String({ description: "The number of the train", examples: "IC 287" })),
		operator: t.Optional(t.Object({
			id: t.Optional(t.String({ description: "The ID of the operator" })),
			name: t.Optional(t.String({ description: "The name of the operator", examples: "Deutsche Bahn" }))
		}))
	})),
	messages: t.Optional(t.Array(MessageSchema)),
	cancelled: t.Optional(t.Boolean({ description: "Indicates if the connection is cancelled." })),
	walking: t.Optional(t.Boolean({ description: "Indicates if the connection includes walking." })),
	distance: t.Optional(t.Number({ description: "The distance of the connection in meters." })),
	loadFactor: t.Optional(t.String({
		description: "The load factor of the connection.",
		examples: ["LOW", "MIDDLE", "HIGH"]
	}))
});
type Connection = typeof ConnectionSchema.static;

export { ConnectionSchema, Connection };

const LineColorSchema = t.Object({
	lineName: t.String({ description: "The name of the line." }),
	hafasLineId: t.String({ description: "The ID of the line in the HAFAS system." }),
	hafasOperatorCode: t.String({ description: "The operator code in the HAFAS system." }),
	backgroundColor: t.String({ description: "The background color of the line." }),
	textColor: t.String({ description: "The text color of the line." }),
	borderColor: t.String({ description: "The border color of the line." }),
	shape: t.String({ description: "The shape of the line." })
}, {
	description: "LineColor represents the color scheme for a specific line in the transportation system. Data obtained from Traewelling"
});
type LineColor = typeof LineColorSchema.static;

export { LineColorSchema, LineColor };