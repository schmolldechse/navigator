import { integer, pgSchema, serial, text, unique } from "drizzle-orm/pg-core";
import { user } from "./auth.schema.ts";

export const dataSchema = pgSchema("data");
export const favoriteStations = dataSchema.table("favorite_stations", {
	id: integer().primaryKey().generatedAlwaysAsIdentity(),
	userId: text("user_id").notNull().references(() => user.id, { onDelete: "cascade" }),
	evaNumber: integer("evaNumber").notNull(),
}, (table) => ({
	uniqueUserEva: unique().on(table.userId, table.evaNumber)
}));