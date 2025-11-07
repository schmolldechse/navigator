//
//  DatabaseSizeMigration.swift
//  backend
//
//  Created by Christian Knapp on 05.11.25.
//

import Fluent
import SQLKit

struct DatabaseSizeMigration: AsyncMigration {
    func prepare(on database: any Database) async throws {
        guard let sql = database as? (any SQLDatabase) else { return }
        
        try await sql.raw("""
            CREATE SCHEMA IF NOT EXISTS "statistics";
        """).run()
        
        try await sql.raw("""
            CREATE TABLE IF NOT EXISTS "\(unsafeRaw: DatabaseSizeHistory.space!)"."\(unsafeRaw: DatabaseSizeHistory.schema)" (
                "id" serial PRIMARY KEY,
                "recorded_at" timestamp NOT NULL DEFAULT now(),
                "size_bytes" bigint NOT NULL
            );
            """).run()
    }
    
    func revert(on database: any Database) async throws {
        guard let sql = database as? (any SQLDatabase) else { return }
        
        try await sql.raw("""
            DROP TABLE IF EXISTS "\(unsafeRaw: DatabaseSizeHistory.space!)"."\(unsafeRaw: DatabaseSizeHistory.schema)";
            """).run()
        
        // schema
        try await sql.raw("""
            DROP SCHEMA IF EXISTS "statistics";
        """).run()
    }
}
