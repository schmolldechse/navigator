CREATE SCHEMA IF NOT EXISTS "auth";
--> statement-breakpoint
CREATE TYPE "auth"."role" AS ENUM('default', 'admin');--> statement-breakpoint
CREATE TABLE "auth"."account" (
	"id" text PRIMARY KEY NOT NULL,
	"accountId" text NOT NULL,
	"providerId" text NOT NULL,
	"userId" text NOT NULL,
	"accessToken" text,
	"refreshToken" text,
	"idToken" text,
	"accessTokenExpiresAt" timestamp,
	"refreshTokenExpiresAt" timestamp,
	"scope" text,
	"password" text,
	"createdAt" timestamp NOT NULL,
	"updatedAt" timestamp NOT NULL
);
--> statement-breakpoint
CREATE TABLE "auth"."session" (
	"id" text PRIMARY KEY NOT NULL,
	"expiresAt" timestamp NOT NULL,
	"token" text NOT NULL,
	"createdAt" timestamp NOT NULL,
	"updatedAt" timestamp NOT NULL,
	"ipAddress" text,
	"userAgent" text,
	"userId" text NOT NULL,
	CONSTRAINT "session_token_unique" UNIQUE("token")
);
--> statement-breakpoint
CREATE TABLE "auth"."user" (
	"id" text PRIMARY KEY NOT NULL,
	"name" text NOT NULL,
	"email" text NOT NULL,
	"emailVerified" boolean NOT NULL,
	"image" text,
	"createdAt" timestamp NOT NULL,
	"updatedAt" timestamp NOT NULL,
	"username" text NOT NULL,
	"role" "auth"."role" DEFAULT 'default' NOT NULL,
	CONSTRAINT "user_email_unique" UNIQUE("email"),
	CONSTRAINT "user_username_unique" UNIQUE("username")
);
--> statement-breakpoint
CREATE TABLE "auth"."verification" (
	"id" text PRIMARY KEY NOT NULL,
	"identifier" text NOT NULL,
	"value" text NOT NULL,
	"expiresAt" timestamp NOT NULL,
	"createdAt" timestamp,
	"updatedAt" timestamp
);
--> statement-breakpoint
CREATE SCHEMA IF NOT EXISTS "core";
CREATE TABLE "core"."journey_messages" (
	"id" serial PRIMARY KEY NOT NULL,
	"journey_id" varchar(82) NOT NULL,
	"journey_date" date NOT NULL,
	"code" varchar(64) NOT NULL,
	"message" varchar(2048) NOT NULL,
	"summary" varchar(2048) NOT NULL
);
--> statement-breakpoint
CREATE TABLE "core"."journey_stop_messages" (
	"id" serial PRIMARY KEY NOT NULL,
	"stop_id" integer NOT NULL,
	"code" varchar(64) NOT NULL,
	"message" varchar(2048) NOT NULL,
	"summary" varchar(2048) NOT NULL
);
--> statement-breakpoint
CREATE TABLE "core"."journey_via-stops" (
	"id" serial PRIMARY KEY NOT NULL,
	"journey_id" varchar(82) NOT NULL,
	"journey_date" date NOT NULL,
	"name" varchar(512) NOT NULL,
	"eva_number" integer NOT NULL,
	"cancelled" boolean NOT NULL,
	"arrival_planned_time" timestamp,
	"arrival_actual_time" timestamp,
	"arrival_delay" integer,
	"arrival_planned_platform" varchar(32),
	"arrival_actual_platform" varchar(32),
	"departure_planned_time" timestamp,
	"departure_actual_time" timestamp,
	"departure_delay" integer,
	"departure_planned_platform" varchar(32),
	"departure_actual_platform" varchar(32)
);
--> statement-breakpoint
CREATE TABLE "core"."journeys" (
	"journey_id" varchar(82) PRIMARY KEY NOT NULL,
	"journey_date" date NOT NULL,
	"product_type" varchar(32) NOT NULL,
	"product_name" varchar(32) NOT NULL,
	"journey_number" varchar(32) NOT NULL,
	"journey_name" varchar(512) NOT NULL,
	"operator_code" varchar(128) NOT NULL,
	"operator_name" varchar(512) NOT NULL
);
--> statement-breakpoint
CREATE TABLE "core"."ris_ids" (
	"id" varchar(73) PRIMARY KEY NOT NULL,
	"product" varchar(128) NOT NULL,
	"discovery_date" timestamp NOT NULL,
	"last_seen" timestamp,
	"last_succeeded_at" timestamp,
	"active" boolean DEFAULT true NOT NULL,
	"is_locked" boolean DEFAULT false NOT NULL
);
--> statement-breakpoint
CREATE TABLE "core"."station_products" (
	"id" serial PRIMARY KEY NOT NULL,
	"eva_number" integer NOT NULL,
	"name" varchar(32) NOT NULL,
	"querying_enabled" boolean NOT NULL
);
--> statement-breakpoint
CREATE TABLE "core"."station_ril100" (
	"id" serial PRIMARY KEY NOT NULL,
	"eva_number" integer NOT NULL,
	"ril100" varchar(32) NOT NULL
);
--> statement-breakpoint
CREATE TABLE "core"."stations" (
	"eva_number" integer PRIMARY KEY NOT NULL,
	"name" varchar(512) NOT NULL,
	"weight" double precision DEFAULT 0 NOT NULL,
	"latitude" double precision NOT NULL,
	"longitude" double precision NOT NULL,
	"querying_enabled" boolean DEFAULT false NOT NULL,
	"last_queried" timestamp,
	"is_locked" boolean DEFAULT false NOT NULL
);
--> statement-breakpoint
CREATE SCHEMA IF NOT EXISTS "user_data";
CREATE TABLE "user_data"."favorite_stations" (
	"id" integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY (sequence name "user_data"."favorite_stations_id_seq" INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START WITH 1 CACHE 1),
	"user_id" text NOT NULL,
	"eva_number" integer NOT NULL,
	CONSTRAINT "favorite_stations_user_id_eva_number_unique" UNIQUE("user_id","eva_number")
);
--> statement-breakpoint
ALTER TABLE "auth"."account" ADD CONSTRAINT "account_userId_user_id_fk" FOREIGN KEY ("userId") REFERENCES "auth"."user"("id") ON DELETE no action ON UPDATE no action;--> statement-breakpoint
ALTER TABLE "auth"."session" ADD CONSTRAINT "session_userId_user_id_fk" FOREIGN KEY ("userId") REFERENCES "auth"."user"("id") ON DELETE no action ON UPDATE no action;--> statement-breakpoint
ALTER TABLE "core"."journey_messages" ADD CONSTRAINT "journey_messages_journey_id_journeys_journey_id_fk" FOREIGN KEY ("journey_id") REFERENCES "core"."journeys"("journey_id") ON DELETE cascade ON UPDATE no action;--> statement-breakpoint
ALTER TABLE "core"."journey_stop_messages" ADD CONSTRAINT "journey_stop_messages_stop_id_journey_via-stops_id_fk" FOREIGN KEY ("stop_id") REFERENCES "core"."journey_via-stops"("id") ON DELETE cascade ON UPDATE no action;--> statement-breakpoint
ALTER TABLE "core"."journey_via-stops" ADD CONSTRAINT "journey_via-stops_journey_id_journeys_journey_id_fk" FOREIGN KEY ("journey_id") REFERENCES "core"."journeys"("journey_id") ON DELETE cascade ON UPDATE no action;--> statement-breakpoint
ALTER TABLE "core"."station_products" ADD CONSTRAINT "station_products_eva_number_stations_eva_number_fk" FOREIGN KEY ("eva_number") REFERENCES "core"."stations"("eva_number") ON DELETE cascade ON UPDATE no action;--> statement-breakpoint
ALTER TABLE "core"."station_ril100" ADD CONSTRAINT "station_ril100_eva_number_stations_eva_number_fk" FOREIGN KEY ("eva_number") REFERENCES "core"."stations"("eva_number") ON DELETE cascade ON UPDATE no action;--> statement-breakpoint
ALTER TABLE "user_data"."favorite_stations" ADD CONSTRAINT "favorite_stations_user_id_user_id_fk" FOREIGN KEY ("user_id") REFERENCES "auth"."user"("id") ON DELETE cascade ON UPDATE no action;--> statement-breakpoint
CREATE INDEX "idx_messages_journeydate" ON "core"."journey_messages" USING btree ("journey_date");--> statement-breakpoint
CREATE INDEX "idx_via-stops_evanumber" ON "core"."journey_via-stops" USING btree ("eva_number");--> statement-breakpoint
CREATE INDEX "idx_via-stops_journeydate" ON "core"."journey_via-stops" USING btree ("journey_date");--> statement-breakpoint
CREATE INDEX "idx_via-stops_evanumber_journeyid" ON "core"."journey_via-stops" USING btree ("eva_number","journey_id");--> statement-breakpoint
CREATE INDEX "idx_via-stops_evanumber_journeydate" ON "core"."journey_via-stops" USING btree ("eva_number","journey_date");--> statement-breakpoint
CREATE INDEX "idx_journey_date" ON "core"."journeys" USING btree ("journey_date");