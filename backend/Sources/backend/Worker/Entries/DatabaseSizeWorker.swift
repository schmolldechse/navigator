//
//  DatabaseSizeWorker.swift
//  backend
//
//  Created by Christian Knapp on 07.11.25.
//

import Vapor
import NIOCore
import SQLKit

fileprivate struct DatabaseSizeQueryResult: Decodable {
    let database_size: Int64
}

final class DatabaseSizeWorker: ScheduledWorker {
    let name = "database-size-worker"
    let interval: TimeAmount = .hours(1)
    
    private let application: Application
    
    required init(application: Application) {
        self.application = application
    }
    
    func execute() async throws {
        try await self.database.raw("""
            VACUUM ANALYZE;
        """).run()
        let result = try await self.database.raw("""
            SELECT COALESCE(SUM(pg_total_relation_size(c.oid))::bigint, 0) AS database_size
            FROM pg_class c
                JOIN pg_namespace n ON n.oid = c.relnamespace
            WHERE n.nspname = 'core'
            AND c.relkind = 'r';
        """).all(decoding: DatabaseSizeQueryResult.self)
        if result.isEmpty { return }
        
        let entry = DatabaseSizeHistory(recordedAt: Date(), sizeBytes: result[0].database_size)
        try await entry.save(on: self.application.db)
    }
    
    private var database: any SQLDatabase {
        guard let sql = application.db as? (any SQLDatabase) else {
            fatalError("DatabaseSizeWorker requires SQL.")
        }
        return sql
    }
}
