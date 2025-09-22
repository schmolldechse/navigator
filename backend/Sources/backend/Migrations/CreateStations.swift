//
//  CreateStations.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent

struct CreateStations: AsyncMigration {
    func prepare(on database: any Database) async throws {
        try await database.schema(Station.schema, space: Station.space)
            .field("eva_number", .int32, .required, .identifier(auto: false))
            .field("name", .custom("VARCHAR(512)"), .required)
            .field("weight", .double)
            .field("latitude", .double, .required)
            .field("longitude", .double, .required)
            .field("querying_enabled", .bool, .required)
            .field("last_queried", .datetime)
            .field("is_locked", .bool, .required)
            .create()
        
        try await database.schema(TransportOccurence.schema, space: TransportOccurence.space)
            .field("id", .int32, .required, .identifier(auto: true))
            .field("eva_number", .int32, .required, .references(Station.schema, space: Station.space, "eva_number", onDelete: .cascade))
            .field("transport_name", .string, .required)
            .field("querying_enabled", .bool, .required)
            .create()
        
        try await database.schema(Ril100.schema, space: Ril100.space)
            .field("id", .int32, .required, .identifier(auto: true))
            .field("eva_number", .int32, .required, .references(Station.schema, space: Station.space, "eva_number", onDelete: .cascade))
            .field("ril100", .custom("VARCHAR(64)"), .required)
            .create()
    }
    
    func revert(on database: any Database) async throws {
        try await database.schema(Station.schema, space: Station.space).delete()
        try await database.schema(TransportOccurence.schema, space: TransportOccurence.space).delete()
        try await database.schema(Ril100.schema, space: Ril100.space).delete()
    }
}
