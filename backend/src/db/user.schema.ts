import { integer, pgSchema, serial, text, unique } from "drizzle-orm/pg-core";

/**
 * TODO: fix
export const userSchema = pgSchema("data");
export const favoriteStations = userSchema.table(
	"favorite_stations",
	{
		id: integer().primaryKey().generatedAlwaysAsIdentity(),
		userId: text("user_id")
			.notNull()
			.references(() => user.id, { onDelete: "cascade" }),
		evaNumber: integer("evaNumber").notNull()
	},
	(table) => ({
		uniqueUserEva: unique().on(table.userId, table.evaNumber)
	})
);
*/