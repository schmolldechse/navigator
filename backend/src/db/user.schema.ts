import { integer, pgSchema, text, unique } from "drizzle-orm/pg-core";
import { user } from "./auth.schema";

const schema = pgSchema("user_data");

const favoriteStations = schema.table(
	"favorite_stations",
	{
		id: integer().primaryKey().generatedAlwaysAsIdentity(),
		userId: text("user_id")
			.notNull()
			.references(() => user.id, { onDelete: "cascade" }),
		evaNumber: integer("eva_number").notNull()
	},
	(table) => [unique().on(table.userId, table.evaNumber)]
);

export { favoriteStations };
