import { t } from "elysia";

const MessageSchema = t.Object(
	{
		type: t.String({ description: "Type of the message" }),
		text: t.Optional(t.String({ description: "Text of the message" })),
		summary: t.Optional(t.String({ description: "Summary of the message" })),
		links: t.Optional(t.Array(t.Any(), { description: "Links related to the message" }))
	},
	{ description: "Message object" }
);

export { MessageSchema };