import { pgSchema, varchar, timestamp, integer, boolean, serial, index, date, doublePrecision } from "drizzle-orm/pg-core";

const coreSchema = pgSchema("core");

// ris_ids
const risIds = coreSchema.table("ris_ids", {
	id: varchar("id", { length: 73 }).primaryKey(),
	product: varchar("product", { length: 128 }).notNull(),
	discoveryDate: timestamp("discovery_date").notNull(),
	lastSeenDate: timestamp("last_seen"),
	lastSucceededAt: timestamp("last_succeeded_at"),
	active: boolean("active").notNull().default(true),
	isLocked: boolean("is_locked").notNull().default(false)
});

// stations
const stations = coreSchema.table("stations", {
	evaNumber: integer("eva_number").primaryKey(),
	name: varchar("name", { length: 512 }).notNull(),
	weight: doublePrecision("weight").notNull().default(0),
	latitude: doublePrecision("latitude").notNull(),
	longitude: doublePrecision("longitude").notNull(),
	queryingEnabled: boolean("querying_enabled").notNull().default(false),
	lastQueried: timestamp("last_queried"),
	isLocked: boolean("is_locked").notNull().default(false)
});

const stationProducts = coreSchema.table("station_products", {
	id: serial("id").primaryKey(),
	evaNumber: integer("eva_number")
		.notNull()
		.references(() => stations.evaNumber, { onDelete: "cascade" }),
	name: varchar("name", { length: 32 }).notNull(),
	queryingEnabled: boolean("querying_enabled").notNull()
}, (table) => [
	index("idx_products_eva_number_name").on(table.evaNumber, table.name),
]);

const stationRil = coreSchema.table("station_ril100", {
	id: serial("id").primaryKey(),
	evaNumber: integer("eva_number")
		.notNull()
		.references(() => stations.evaNumber, { onDelete: "cascade" }),
	ril100: varchar("ril100", { length: 32 }).notNull()
});

// journeys
const journeys = coreSchema.table(
	"journeys",
	{
		journeyId: varchar("journey_id", { length: 82 }).primaryKey(),
		journeyDate: date("journey_date").notNull(),
		productType: varchar("product_type", { length: 32 }).notNull(),
		productName: varchar("product_name", { length: 32 }).notNull(),
		journeyNumber: varchar("journey_number", { length: 32 }).notNull(),
		journeyName: varchar("journey_name", { length: 512 }).notNull(),
		operatorCode: varchar("operator_code", { length: 128 }).notNull(),
		operatorName: varchar("operator_name", { length: 512 }).notNull()
	},
	(table) => [
		index("idx_journey_date").on(table.journeyDate),
		index("idx_journey_product_type").on(table.productType),
		index("idx_journey_date_product_type").on(table.journeyDate, table.productType)
	]
);

const journeyMessages = coreSchema.table(
	"journey_messages",
	{
		id: serial("id").primaryKey(),
		journeyId: varchar("journey_id", { length: 82 })
			.notNull()
			.references(() => journeys.journeyId, { onDelete: "cascade" }),
		journeyDate: date("journey_date").notNull(),
		code: varchar("code", { length: 64 }).notNull(),
		message: varchar("message", { length: 2048 }).notNull(),
		summary: varchar("summary", { length: 2048 }).notNull()
	},
	(table) => [index("idx_messages_journeydate").on(table.journeyDate)]
);

const journeyViaStops = coreSchema.table(
	"journey_via-stops",
	{
		id: serial("id").primaryKey(),
		journeyId: varchar("journey_id", { length: 82 })
			.notNull()
			.references(() => journeys.journeyId, { onDelete: "cascade" }),
		journeyDate: date("journey_date").notNull(),
		name: varchar("name", { length: 512 }).notNull(),
		evaNumber: integer("eva_number").notNull(),
		cancelled: boolean("cancelled").notNull(),
		// flattened arrival
		arrivalPlannedTime: timestamp("arrival_planned_time"),
		arrivalActualTime: timestamp("arrival_actual_time"),
		arrivalDelay: integer("arrival_delay"),
		arrivalPlannedPlatform: varchar("arrival_planned_platform", { length: 32 }),
		arrivalActualPlatform: varchar("arrival_actual_platform", { length: 32 }),
		// flattened departure
		departurePlannedTime: timestamp("departure_planned_time"),
		departureActualTime: timestamp("departure_actual_time"),
		departureDelay: integer("departure_delay"),
		departurePlannedPlatform: varchar("departure_planned_platform", { length: 32 }),
		departureActualPlatform: varchar("departure_actual_platform", { length: 32 })
	},
	(table) => [
		index("idx_via-stops_evanumber").on(table.evaNumber),
		index("idx_via-stops_journeydate").on(table.journeyDate),
		index("idx_via-stops_evanumber_journeyid").on(table.evaNumber, table.journeyId),
		index("idx_via-stops_evanumber_journeydate").on(table.evaNumber, table.journeyDate)
	]
);

const journeyStopMessages = coreSchema.table("journey_stop_messages", {
	id: serial("id").primaryKey(),
	stopId: integer("stop_id")
		.notNull()
		.references(() => journeyViaStops.id, { onDelete: "cascade" }),
	code: varchar("code", { length: 64 }).notNull(),
	message: varchar("message", { length: 2048 }).notNull(),
	summary: varchar("summary", { length: 2048 }).notNull()
});

export { risIds, stations, stationProducts, stationRil, journeys, journeyMessages, journeyViaStops, journeyStopMessages };
