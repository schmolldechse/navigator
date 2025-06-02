import { pgSchema, uuid, varchar, timestamp, integer, boolean, serial } from "drizzle-orm/pg-core";

const coreSchema = pgSchema("core");

// ris_ids
const risIds = coreSchema.table("ris_ids", {
	id: uuid("id").notNull().primaryKey(),
	product: varchar("product", { length: 128 }).notNull(),
	discoveryDate: timestamp("discovery_date", { mode: "date" }).notNull(),
	lastSeenDate: timestamp("last_seen", { mode: "date" }),
	lastSucceededAt: timestamp("last_succeeded_at", { mode: "date" }),
});

// stations
const stations = coreSchema.table("stations", {
	evaNumber: integer("eva_number").notNull().primaryKey(),
	name: varchar("name", { length: 512 }).notNull(),
	latitude: integer("latitude").notNull(),
	longitude: integer("longitude").notNull(),
	queryingEnabled: boolean("querying_enabled"),
	lastQueried: timestamp("last_queried", { mode: "date" }),
});

const stationProducts = coreSchema.table("station_products", {
	id: serial("id").primaryKey(),
	evaNumber: integer("eva_number").notNull().references(() => stations.evaNumber, { onDelete: "cascade" }),
	name: varchar("name", { length: 32 }).notNull(),
	queryingEnabled: boolean("querying_enabled").notNull(),
});

const stationRil = coreSchema.table("station_ril100", {
	id: serial("id").primaryKey(),
	evaNumber: integer("eva_number").notNull().references(() => stations.evaNumber, { onDelete: "cascade" }),
	ril100: varchar("ril100", { length: 32 }).notNull(),
});

export { risIds, stations, stationProducts, stationRil };
