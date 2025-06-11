import { pgSchema, uuid, varchar, timestamp, integer, boolean, serial } from "drizzle-orm/pg-core";

const coreSchema = pgSchema("core");

// ris_ids
const risIds = coreSchema.table("ris_ids", {
	id: uuid("id").notNull().primaryKey(),
	product: varchar("product", { length: 128 }).notNull(),
	discoveryDate: timestamp("discovery_date", { mode: "date" }).notNull(),
	lastSeenDate: timestamp("last_seen", { mode: "date" }),
	lastSucceededAt: timestamp("last_succeeded_at", { mode: "date" }),
	active: boolean("active").notNull().default(true),
});

// stations
const stations = coreSchema.table("stations", {
	evaNumber: integer("eva_number").notNull().primaryKey(),
	name: varchar("name", { length: 512 }).notNull(),
	weight: integer("weight").notNull().default(0),
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

// journeys
const journeys = coreSchema.table("journeys", {
	journeyId: varchar("journey_id", { length: 82 }).notNull().primaryKey(),
	productType: varchar("product_type", { length: 32 }).notNull(),
	productName: varchar("product_name", { length: 32 }).notNull(),
	journeyNumber: varchar("journey_number", { length: 32 }).notNull(),
	journeyName: varchar("journey_name", { length: 512 }).notNull(),
});

const journeyMessage = coreSchema.table("journey_messages", {
	id: serial("id").primaryKey(),
	journeyId: varchar("journey_id", { length: 82 }).notNull().references(() => journeys.journeyId, { onDelete: "cascade" }),
	code: integer("code").notNull(),
	message: varchar("message", { length: 2048 }).notNull(),
	summary: varchar("summary", { length: 2048 }).notNull(),
});

const journeyViaStops = coreSchema.table("journey_via-stops", {
	id: serial("id").primaryKey(),
	journeyId: varchar("journey_id", { length: 82 }).notNull().references(() => journeys.journeyId, { onDelete: "cascade" }),
	evaNumber: integer("eva_number").notNull(),
	cancelled: boolean("cancelled").notNull(),
	arrivalPlannedTime: timestamp("arrival_planned_time", { mode: "date" }),
	arrivalActualTime: timestamp("arrival_actual_time", { mode: "date" }),
	arrivalDelay: integer("arrival_delay"),
	arrivalPlannedPlatform: varchar("arrival_planned_platform", { length: 32 }),
	arrivalActualPlatform: varchar("arrival_actual_platform", { length: 32 }),
	departurePlannedTime: timestamp("departure_planned_time", { mode: "date" }),
	departureActualTime: timestamp("departure_actual_time", { mode: "date" }),
	departureDelay: integer("departure_delay"),
	departurePlannedPlatform: varchar("departure_planned_platform", { length: 32 }),
	departureActualPlatform: varchar("departure_actual_platform", { length: 32 }),
});

const journeyStopMessages = coreSchema.table("journey_stop_messages", {
	id: serial("id").primaryKey(),
	stopId: integer("stop_id").notNull().references(() => journeyViaStops.id, { onDelete: "cascade" }),
	evaNumber: integer("eva_number").notNull(),
	code: integer("code").notNull(),
	message: varchar("message", { length: 2048 }).notNull(),
	summary: varchar("summary", { length: 2048 }).notNull(),
});

export { risIds, stations, stationProducts, stationRil, journeys, journeyMessage, journeyViaStops, journeyStopMessages };
