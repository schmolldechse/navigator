//
//  CoreSchemaCreation.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent
import SQLKit

struct CoreSchemaCreation: AsyncMigration {
    func prepare(on database: any Database) async throws {
        guard let sql = database as? (any SQLDatabase) else { return }
        try await sql.raw("CREATE SCHEMA IF NOT EXISTS core").run()
    }
    
    func revert(on database: any Database) async throws {
        guard let sql = database as? (any SQLDatabase) else { return }
        try await sql.raw("DROP SCHEMA IF EXISTS core").run()
    }
}
