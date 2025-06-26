import { t } from "elysia";
import { TimeSchema } from "./time.model";
import { BasicStationSchema } from "./station.model";

const OccupancySchema = t.Object({
	class: t.UnionEnum(["FIRST_CLASS", "SECOND_CLASS"], {
		description: "Which class the occupancy applies to"
	}),
	level: t.Number({ description: "The level of the occupancy", minimum: 0, maximum: 5 }),
	title: t.String({ description: "Title of the occupancy" }),
	description: t.Optional(t.String({ description: "Description of the occupancy" }))
});

const RouteMessageSchema = t.Object({
	type: t.String({ description: "Type of the message" }),
	content: t.String({ description: "Content of the message" }),
	modificationDate: t.Optional(t.String({ description: "Modification date", format: "date-time" }))
});

const BasicRouteStopSchema = t.Intersect([
	BasicStationSchema,
	t.Object({
		arrival: t.Optional(TimeSchema), // optional arrival on the first stop
		departure: t.Optional(TimeSchema) // optional departure on the last stop
	})
]);

// WALKING SECTION
const WalkingRouteSectionSchema = t.Object({
	isWalking: t.Boolean({ description: "Indicates if the section is a walking section", default: true }),
	distance: t.Number({ description: "Distance to travel between two stops in meters" }),
	origin: BasicRouteStopSchema,
	destination: BasicRouteStopSchema,
	cancelled: t.Boolean({ description: "Indicates if the section is cancelled" }),
	messages: t.Array(RouteMessageSchema, { description: "Array of messages for the section" })
});

// NORMAL SECTION
const ExtendedRouteStopSchema = t.Intersect([
	BasicRouteStopSchema,
	t.Object({
		occupancy: t.Array(OccupancySchema, { description: "Array of occupancy information" }),
		messages: t.Array(RouteMessageSchema),
		cancelled: t.Boolean({ description: "Indicates if the stop is cancelled" })
	})
]);

const RouteJourneyAttributeSchema = t.Object({
	category: t.UnionEnum(["INFORMATION", "ACCESSIBILITY", "BICYCLE-TRANSPORT"], { description: "Category of the attribute" }),
	value: t.String({ description: "Value of the attribute", examples: ["Klimaanlage"] }),
	sectionsNote: t.Optional(t.String({ description: "Note for the section", examples: ["(Köln Hbf - Stuttgart Hbf)"] }))
});

const NormalRouteSectionSchema = t.Object({
	hafas_journeyId: t.String({
		description: "Identifier for the journey in the HAFAS system. Optional when `section` is set to true."
	}),
	origin: ExtendedRouteStopSchema,
	destination: ExtendedRouteStopSchema,
	journeysDirection: t.Optional(BasicStationSchema),
	viaStops: t.Array(ExtendedRouteStopSchema, { description: "Array of via stops. Null if it's a walking section." }),
	lineInformation: t.Object(
		{
			productType: t.String({ description: "The product type of the journey", examples: ["NahverkehrsonstigeZuege"] }),
			productName: t.String({ description: "The product category of the journey", examples: ["RB"] }),
			journeyNumber: t.Number({ description: "The unique id of the journey per day", examples: [22565] }),
			journeyName: t.String({ description: "The name of this journey", examples: ["RB 63"] }),
			operator: t.String({ description: "The operator's name of the journey", examples: ["DB Regio AG"] })
		},
		{ description: "Information about the line of the section. Null if it's a walking section." }
	),
	attributes: t.Array(RouteJourneyAttributeSchema),
	isWalking: t.Boolean({ description: "Indicates if the section is a walking section", default: false }),
	occupancy: t.Array(OccupancySchema, { description: "Array of occupancy information" }),
	messages: t.Array(RouteMessageSchema, { description: "Array of messages for the section" }),
	cancelled: t.Boolean({ description: "Indicates if the section is cancelled" })
});

const RouteEntrySchema = t.Object({
	refreshToken: t.String({ description: "Token to refresh the route" }),
	sections: t.Array(t.Union([NormalRouteSectionSchema, WalkingRouteSectionSchema]), {
		description: "Array of route sections"
	}),
	isAlternative: t.Boolean({ description: "Indicates if the route is an alternative route" }),
	occupancy: t.Array(OccupancySchema, { description: "Array of occupancy information" }),
	messages: t.Array(RouteMessageSchema, { description: "Array of messages for the route" })
});

const RouteDetailsSchema = t.Object({
	earlierRef: t.String({ description: "Reference to lookup for earlier routes." }),
	laterRef: t.String({ description: "Reference to lookup for later routes." }),
	entries: t.Array(RouteEntrySchema, { description: "Array of routes" })
});

export {
	OccupancySchema,
	RouteMessageSchema,
	BasicRouteStopSchema,
	ExtendedRouteStopSchema,
	WalkingRouteSectionSchema,
	RouteJourneyAttributeSchema,
	NormalRouteSectionSchema,
	RouteEntrySchema,
	RouteDetailsSchema
};
