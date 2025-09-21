//
//  CreateIdentifiedRISId.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent

struct CreateIdentifiedRISId: AsyncMigration {
    func prepare(on database: any Database) async throws {
        let transportTypeEnum = try await database.enum("TransportType").read()
        
        try await database.schema(IdentifiedRISID.schema, space: IdentifiedRISID.space)
            .field("id", .string, .required, .identifier(auto: false))
            .field("transport_type", transportTypeEnum, .required)
            .field("replacement_transport_type", transportTypeEnum)
            .field("discovery_date", .datetime, .required)
            .field("last_seen", .datetime)
            .field("last_succeeded_at", .datetime)
            .field("active", .bool, .required)
            .field("is_locked", .bool, .required)
            .create()
    }
    
    func revert(on database: any Database) async throws {
        try await database.schema(IdentifiedRISID.schema, space: IdentifiedRISID.space).delete()
    }
}
