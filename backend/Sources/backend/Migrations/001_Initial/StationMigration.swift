//
//  StationMigration.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent
import SQLKit

struct StationMigration: AsyncMigration {
    func prepare(on database: any Database) async throws {
        guard let sql = database as? (any SQLDatabase) else { return }
        
        // `cube` and `earthdistance` extensions
        try await sql.raw("""
            CREATE EXTENSION IF NOT EXISTS cube;
            """).run()
        try await sql.raw("""
            CREATE EXTENSION IF NOT EXISTS earthdistance;
            """).run()
                
        // stations
        try await sql.raw("""
            CREATE TABLE IF NOT EXISTS "\(bind: Station.space!)"."\(bind: Station.schema)" (
                "eva_number" integer PRIMARY KEY NOT NULL,
                "name" varchar(512) NOT NULL,
                "weight" double precision DEFAULT 0 NOT NULL,
                "latitude" double precision NOT NULL,
                "longitude" double precision NOT NULL,
                "querying_enabled" boolean DEFAULT false NOT NULL,
                "last_queried" timestamp,
                "is_locked" boolean DEFAULT false NOT NULL
            );
            """).run()
        try await sql.raw("""
            CREATE INDEX "idx_stations_location" ON "\(bind: Station.space!)"."\(bind: Station.schema)" USING gist (ll_to_earth(latitude, longitude));
            """).run()
        
        // station_transports
        try await sql.raw("""
            CREATE TABLE IF NOT EXISTS "\(bind: TransportOccurence.space!)"."\(bind: TransportOccurence.schema)" (
                "id" serial PRIMARY KEY NOT NULL,
                "eva_number" integer NOT NULL REFERENCES "\(bind: Station.space!)"."\(bind: Station.schema)" (eva_number) ON DELETE CASCADE,
                "transport_name" varchar(255) NOT NULL,
                "querying_enabled" boolean DEFAULT false NOT NULL
            );
            """).run()
        try await sql.raw("""
            CREATE INDEX "idx_eva_number_transport" ON "\(bind: TransportOccurence.space!)"."\(bind: TransportOccurence.schema)" (eva_number, transport_name)
            """).run()
            
        // station_ril100
        try await sql.raw("""
            CREATE TABLE IF NOT EXISTS "\(bind: Ril100.space!)"."\(bind: Ril100.schema)" (
                "id" serial PRIMARY KEY NOT NULL,
                "eva_number" integer NOT NULL REFERENCES "\(bind: Station.space!)"."\(bind: Station.schema)" (eva_number) ON DELETE CASCADE,
                "ril100" varchar(64) NOT NULL
            );
            """).run()
    }
    
    func revert(on database: any Database) async throws {
        guard let sql = database as? (any SQLDatabase) else { return }
        
        // station_ril100
        try await sql.raw("""
            DROP TABLE IF EXISTS "\(bind: Ril100.space!)"."\(bind: Ril100.schema)";
            """).run()
        
        // station_transports
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_eva_number_transport";
            """).run()
        try await sql.raw("""
            DROP TABLE IF EXISTS "\(bind: TransportOccurence.space!)"."\(bind: TransportOccurence.schema)";
            """).run()
        
        // stations
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_stations_location";
            """).run()
        try await sql.raw("""
            DROP TABLE IF EXISTS "\(bind: Station.space!)"."\(bind: Station.schema)";
            """).run()
    }
}
