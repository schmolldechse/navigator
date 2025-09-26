//
//  IdentifiedRISIDMigration.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent
import SQLKit

struct IdentifiedRISIDMigration: AsyncMigration {
    func prepare(on database: any Database) async throws {
        guard let sql = database as? (any SQLDatabase) else { return }
        try await sql.raw("""
            CREATE TABLE IF NOT EXISTS "\(unsafeRaw: IdentifiedRISID.space!)"."\(unsafeRaw: IdentifiedRISID.schema)" (
                id varchar(128) PRIMARY KEY NOT NULL,
                transport_type varchar(255) NOT NULL,
                replacement_transport_type varchar(255),
                discovery_date timestamp NOT NULL,
                last_seen timestamp,
                last_succeeded_at timestamp,
                active boolean DEFAULT true NOT NULL,
                is_locked boolean DEFAULT false NOT NULL
            );
            """).run()
        try await sql.raw("""
            CREATE INDEX IF NOT EXISTS "idx_ris_id_transport_type" ON "\(unsafeRaw: IdentifiedRISID.space!)"."\(unsafeRaw: IdentifiedRISID.schema)" (transport_type);
            """).run()
    }
    
    func revert(on database: any Database) async throws {
        guard let sql = database as? (any SQLDatabase) else { return }
        try await sql.raw("""
            DROP INDEX IF EXISTS "idx_ris_id_transport_type";
            """).run()
        try await sql.raw("""
            DROP TABLE IF EXISTS "\(unsafeRaw: IdentifiedRISID.space!)"."\(unsafeRaw: IdentifiedRISID.schema)";
            """).run()
    }
}
