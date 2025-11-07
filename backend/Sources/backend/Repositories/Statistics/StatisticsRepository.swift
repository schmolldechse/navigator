//
// StatisticsRepository.swift
// backend
//
// Created by Christian Knapp on 07.11.25.
//

import Vapor
import Fluent
import SQLKit

fileprivate struct TotalDatabaseSizeQueryResult: Decodable {
    let database_size: Int64
}

struct StatisticsRepository {
    let client: any Client
    let logger: Logger
    let database: any Database
    
    func estimateSize(request: EstimateDatabaseSizeByTimeframeDTO) async throws -> MeasuredTimeframeStatisticDTO {
        guard let sql = self.database as? (any SQLDatabase) else {
            logger.error("`estimateSize(request)` requires SQL.")
            throw Abort(.internalServerError, reason: "`estimateSize(request)` requires SQL.")
        }
        
        try await sql.raw("""
            VACUUM ANALYZE;
        """).run()
        let totalDatabaseSize = try await sql.raw("""
            SELECT COALESCE(SUM(pg_total_relation_size(c.oid))::bigint, 0) AS database_size
            FROM pg_class c
                JOIN pg_namespace n ON n.oid = c.relnamespace
            WHERE n.nspname = 'core'
            AND c.relkind = 'r';
        """).all(decoding: TotalDatabaseSizeQueryResult.self)
            
        let sizeHistory = try await DatabaseSizeHistory.query(on: self.database)
            .filter(\.$recordedAt >= request.start)
            .filter(\.$recordedAt <= request.end)
            .sort(\.$recordedAt, .ascending)
            .all()
                
        let currentSize = Int64(totalDatabaseSize.first?.database_size ?? 0)
        let startSize = sizeHistory.first?.sizeBytes ?? 0
        
        var values: [MeasuredValueDTO] = sizeHistory.compactMap {
            MeasuredValueDTO(date: $0.recordedAt, value: $0.sizeBytes)
        }
        values.append(MeasuredValueDTO(date: Date(), value: currentSize))
        values.sort(by: { $0.date < $1.date })
        
        return MeasuredTimeframeStatisticDTO(
            timeframe: DateRangeDTO(start: request.start, end: request.end),
            total: currentSize,
            change: currentSize - startSize,
            unit: .bytes,
            values: values
        )
    }
}
