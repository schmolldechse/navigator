//
//  CreateStations.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent

struct CreateStations: AsyncMigration {
    func prepare(on database: any Database) async throws {
        let transportTypeEnum = try await database.enum("TransportType")
            .case("HIGH_SPEED_TRAIN")
            .case("INTERCITY_TRAIN")
            .case("INTER_REGIONAL_TRAIN")
            .case("REGIONAL_TRAIN")
            .case("CITY_TRAIN")
            .case("SUBWAY")
            .case("TRAM")
            .case("BUS")
            .case("FERRY")
            .case("FLIGHT")
            .case("CAR")
            .case("TAXI")
            .case("SHUTTLE")
            .case("BIKE")
            .case("SCOOTER")
            .case("WALK")
            .case("UNKNOWN")
            .create()
        
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
            .id()
            .field("transport_name", transportTypeEnum, .required)
            .field("querying_enabled", .bool, .required)
            .field("eva_number", .int32, .required, .references(Station.schema, space: Station.space, "eva_number", onDelete: .cascade))
            .create()
        
        try await database.schema(Ril100.schema, space: Ril100.space)
            .id()
            .field("ril100", .string, .required)
            .field("eva_number", .int32, .required, .references(Station.schema, space: Station.space, "eva_number", onDelete: .cascade))
            .create()
    }
    
    func revert(on database: any Database) async throws {
        try await database.enum("TransportType").delete()
        try await database.schema(Station.schema, space: Station.space).delete()
        try await database.schema(TransportOccurence.schema, space: TransportOccurence.space).delete()
        try await database.schema(Ril100.schema, space: Ril100.space).delete()
    }
}
