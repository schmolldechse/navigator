//
//  JourneyMigration.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent
import SQLKit

struct JourneyMigration: AsyncMigration {
    func prepare(on database: any Database) async throws {
        guard let sql = database as? (any SQLDatabase) else { return }
        
        // journey_administrations
        try await sql.raw("""
            CREATE TABLE IF NOT EXISTS "\(bind: Administration.space!)"."\(bind: Administration.schema)" (
                id serial PRIMARY KEY NOT NULL,
                administration_id varchar(64) NOT NULL,
                operator_code varchar(64) NOT NULL,
                operator_name varchar(256) NOT NULL,
                CONSTRAINT unique_administrations UNIQUE (administration_id, operator_code, operator_name)
            );
            """).run()
        
        // journeys
        try await sql.raw("""
            CREATE TABLE IF NOT EXISTS "\(bind: Journey.space!)"."\(bind: Journey.schema)" (
                journey_id varchar(128) PRIMARY KEY NOT NULL,
                date date NOT NULL,
                inserted_at timestamp NOT NULL,
                type varchar(255) NOT NULL,
                administration_index integer NOT NULL REFERENCES "\(bind: Administration.space!)"."\(bind: Administration.schema)" (id) ON DELETE CASCADE,
                cancelled boolean NOT NULL DEFAULT false
            );
            """).run()
        try await sql.raw("""
            CREATE INDEX IF NOT EXISTS "idx_journeys_date" ON "\(bind: Journey.space!)"."\(bind: Journey.schema)" (date);
            """).run()
        try await sql.raw("""
            CREATE INDEX IF NOT EXISTS "idx_journeys_cancelled" ON "\(bind: Journey.space!)"."\(bind: Journey.schema)" (cancelled);
            """).run()
        
        // journey_transports
        try await sql.raw("""
            CREATE TABLE IF NOT EXISTS "\(bind: Transport.space!)"."\(bind: Transport.schema)" (
                journey_id varchar(128) PRIMARY KEY NOT NULL REFERENCES "\(bind: Journey.space!)"."\(bind: Journey.schema)" (journey_id) ON DELETE CASCADE,
                type varchar(255) NOT NULL,
                replacement_type varchar(255),
                category varchar(64) NOT NULL,
                category_internal varchar(64) NOT NULL,
                journey_description varchar(128) NOT NULL,
                label varchar(128) NOT NULL,
                number integer NOT NULL,
                line varchar(64)
            );
            """).run()
        try await sql.raw("""
            CREATE INDEX IF NOT EXISTS "idx_transports_type" ON "\(bind: Transport.space!)"."\(bind: Transport.schema)" (type);
            """).run()
        try await sql.raw("""
            CREATE INDEX IF NOT EXISTS "idx_transports_replacement_type" ON "\(bind: Transport.space!)"."\(bind: Transport.schema)" (replacement_type);
            """).run()
        try await sql.raw("""
            CREATE INDEX IF NOT EXISTS "idx_transports_category" ON "\(bind: Transport.space!)"."\(bind: Transport.schema)" (category);
            """).run()
        try await sql.raw("""
            CREATE INDEX IF NOT EXISTS "idx_transports_journey_description" ON "\(bind: Transport.space!)"."\(bind: Transport.schema)" (journey_description);
            """).run()
        try await sql.raw("""
            CREATE INDEX IF NOT EXISTS "idx_transports_number" ON "\(bind: Transport.space!)"."\(bind: Transport.schema)" (number);
            """).run()
        
        // journey_scheduled_stop_places
        try await sql.raw("""
            CREATE TABLE IF NOT EXISTS "\(bind: ScheduleAtStopPlace.space!)"."\(bind: ScheduleAtStopPlace.schema)" (
                id serial PRIMARY KEY NOT NULL,
                journey_id varchar(128) NOT NULL REFERENCES "\(bind: Journey.space!)"."\(bind: Journey.schema)" (journey_id) ON DELETE CASCADE,
                date date NOT NULL,
                type varchar(255) NOT NULL,
                station_name varchar(512) NOT NULL,
                station_eva_number integer NOT NULL,
                cancelled boolean NOT NULL,
                additional boolean NOT NULL,
                demand boolean NOT NULL,
                no_passenger_change boolean NOT NULL,
                planned_time timestamp NOT NULL,
                actual_time timestamp NOT NULL,
                delay integer NOT NULL,
                planned_platform varchar(32),
                actual_platform varchar(32)
            );
            """).run()
        try await sql.raw("""
            CREATE INDEX IF NOT EXISTS "idx_scheduled_stop_places_date" ON "\(bind: ScheduleAtStopPlace.space!)"."\(bind: ScheduleAtStopPlace.schema)" (date);
            """).run()
        try await sql.raw("""
            CREATE INDEX IF NOT EXISTS "idx_scheduled_stop_places_eva_number" ON "\(bind: ScheduleAtStopPlace.space!)"."\(bind: ScheduleAtStopPlace.schema)" (station_eva_number);
            """).run()
        try await sql.raw("""
            CREATE INDEX IF NOT EXISTS "idx_scheduled_stop_places_eva_number_journey_id" ON "\(bind: ScheduleAtStopPlace.space!)"."\(bind: ScheduleAtStopPlace.schema)" (station_eva_number, journey_id);
            """).run()
        try await sql.raw("""
            CREATE INDEX IF NOT EXISTS "idx_scheduled_stop_places_eva_number_date" ON "\(bind: ScheduleAtStopPlace.space!)"."\(bind: ScheduleAtStopPlace.schema)" (station_eva_number, date);
            """).run()
        
        // journey_stop_place_informations
        try await sql.raw("""
            CREATE TABLE IF NOT EXISTS "\(bind: Information.space!)"."\(bind: Information.schema)" (
                id serial PRIMARY KEY NOT NULL,
                scheduled_stop_place_id integer NOT NULL REFERENCES "\(bind: ScheduleAtStopPlace.space!)"."\(bind: ScheduleAtStopPlace.schema)" (id) ON DELETE CASCADE,
                type varchar(255) NOT NULL,
                key varchar(128) NOT NULL,
                text varchar(2048) NOT NULL,
                text_short varchar(2048),
                disruption_communication_id varchar(255),
                disruption_id varchar(255)
            );
            """).run()
    }
    
    func revert(on database: any Database) async throws {
        guard let sql = database as? (any SQLDatabase) else { return }
        
        // journey_stop_place_informations
        try await sql.raw("""
            DROP TABLE IF EXISTS "\(bind: Information.space!)"."\(bind: Information.schema)";
            """).run()
        
        // journey_scheduled_stop_places
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_scheduled_stop_places_eva_number_date";
            """).run()
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_scheduled_stop_places_eva_number_journey_id";
            """).run()
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_scheduled_stop_places_eva_number";
            """).run()
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_scheduled_stop_places_date";
            """).run()
        try await sql.raw("""
            DROP TABLE IF EXISTS "\(bind: ScheduleAtStopPlace.space!)"."\(bind: ScheduleAtStopPlace.schema)";
            """).run()
        
        // journey_transports
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_transports_number";
            """).run()
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_transports_journey_description";
            """).run()
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_transports_category";
            """).run()
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_transports_replacement_type";
            """).run()
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_transports_type";
            """).run()
        try await sql.raw("""
            DROP TABLE IF EXISTS "\(bind: Transport.space!)"."\(bind: Transport.schema)";
            """).run()
        
        // journeys
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_journeys_date";
            """).run()
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_journeys_cancelled";
            """).run()
        try await sql.raw("""
            DROP TABLE IF EXISTS "\(bind: Journey.space!)"."\(bind: Journey.schema)";
            """).run()
        
        // journey_administrations
        try await sql.raw("""
            DROP TABLE IF EXISTS "\(bind: Administration.space!)"."\(bind: Administration.schema)";
            """).run()
    }
}
