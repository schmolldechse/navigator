import { t } from "elysia";

const MessageSchema = t.Object({
	type: t.String({ description: "Type of the message" }),
	text: t.Optional(t.String({ description: "Text of the message" })),
	summary: t.Optional(t.String({ description: "Summary of the message" })),
	links: t.Optional(t.Array(t.Any(), { description: "Links related to the message" }))
}, { description: "Message object" });
type Message = typeof MessageSchema.static;

export { MessageSchema, type Message };

/**
 * "links" RIS Examples:
 *
 * Label: [{"type":"link","label":"+43 517 17","href":"tel:+4351717","openInNewTab":false}]
 * Station: [{"type":"station","name":"Norddeich Mole","evaNumber":"8007768","slug":"norddeich-mole","nameParts":[{"type":"city","value":"Norddeich"},{"type":"city-separator","value":" "},{"type":"default","value":"Mole"}]}]
 * Replacement Service: [{"type":"replacement-service-map","label":"Wo fährt der Bus?"},{"type":"line","lineName":"ICE 519","transportType":"HIGH_SPEED_TRAIN","isCanceled":false,"isReplacementBusService":false}]
 *
 * HAFAS Examples:
 * [{"text":"Wifi available","type": "hint", "code": "wifi", "summary": "WiFi available"}]
 */
