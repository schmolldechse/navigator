import { t } from "elysia";
import { ConnectionSchema } from "./connection";
import { MessageSchema } from "./message";

const RouteDataSchema = t.Object({
	earlierRef: t.String({ description: "Reference to lookup for earlier routes." }),
	laterRef: t.String({ description: "Reference to lookup for later routes." }),
	journeys: t.Array(t.Object({
		legs: t.Array(ConnectionSchema),
		messages: t.Array(MessageSchema),
		refreshToken: t.Optional(t.String({ description: "Reference to load data specific for this route." }))
	}), { description: "Array of routes" })
});
type RouteData = typeof RouteDataSchema.static;

export { RouteDataSchema, RouteData };