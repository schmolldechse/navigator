//
//  CreateJourneys.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent

struct CreateJourneys: AsyncMigration {
    func prepare(on database: any Database) async throws {
        // Enums
        let journeyTypeEnum = try await database.enum("JourneyType")
            .case("REGULAR")
            .case("REPLACEMENT")
            .case("RELIEF")
            .case("EXTRA")
            .create()
        
        let scheduleTypeEnum = try await database.enum("ScheduleType")
            .case("ARRIVAL")
            .case("DEPARTURE")
            .create()
        
        let informationTypeEnum = try await database.enum("InformationType")
            .case("JOURNEY_ATTRIBUTE")
            .case("DISRUPTION")
            .case("MESSAGE")
            .case("RIS_QUALITY_DEVIATION")
            .case("RIS_CAUSE_REASON")
            .create()
        
        let transportTypeEnum = try await database.enum("TransportType").read()
        
        // Tables
        try await database.schema(Administration.schema, space: Administration.space)
            .field("id", .int, .identifier(auto: true))
            .field("administration_id", .custom("VARCHAR(128)"), .required)
            .field("operator_code", .custom("VARCHAR(64)"), .required)
            .field("operator_name", .custom("VARCHAR(256)"), .required)
            .unique(on: "administration_id", "operator_code", "operator_name", name: "admin_admin_id_op_code_op_name_uidx")
            .create()
        
        try await database.schema(Journey.schema, space: Journey.space)
            .field("journey_id", .custom("VARCHAR(128)"), .identifier(auto: false))
            .field("date", .date, .required)
            .field("inserted_at", .datetime, .required)
            .field("type", journeyTypeEnum, .required)
            .field("administration_index", .int, .required, .references(Administration.schema, space: Administration.space, "id", onDelete: .cascade))
            .create()
        
        try await database.schema(Transport.schema, space: Transport.space)
            .field("journey_id", .custom("VARCHAR(128)"), .identifier(auto: false))
            .foreignKey("journey_id", references: Journey.schema, inSpace: Journey.space, "journey_id", onDelete: .cascade)
            .field("type", transportTypeEnum, .required)
            .field("replacement_type", transportTypeEnum)
            .field("category", .custom("VARCHAR(64)"), .required)
            .field("category_internal", .custom("VARCHAR(64)"), .required)
            .field("journey_description", .custom("VARCHAR(128)"), .required)
            .field("label", .custom("VARCHAR(128)"), .required)
            .field("number", .int32, .required)
            .field("line", .custom("VARCHAR(64)"))
            .create()
        
        try await database.schema(ScheduleAtStopPlace.schema, space: ScheduleAtStopPlace.space)
            .field("id", .int, .identifier(auto: true))
            .field("journey_id", .custom("VARCHAR(128)"), .required, .references(Journey.schema, space: Journey.space, "journey_id", onDelete: .cascade))
            .field("date", .date, .required)
            .field("type", scheduleTypeEnum, .required)
            .field("station_name", .string, .required)
            .field("station_eva_number", .int32, .required)
            .field("cancelled", .bool, .required)
            .field("additional", .bool, .required)
            .field("demand", .bool, .required)
            .field("no_passenger_change", .bool, .required)
            .field("planned_time", .datetime, .required)
            .field("actual_time", .datetime, .required)
            .field("delay", .int32, .required)
            .field("planned_platform", .custom("VARCHAR(32)"))
            .field("actual_platform", .custom("VARCHAR(32)"))
            .create()
        
        try await database.schema(Information.schema, space: Information.space)
            .field("id", .int, .identifier(auto: true))
            .field("scheduled_stop_place_id", .int, .required, .references(ScheduleAtStopPlace.schema, space: ScheduleAtStopPlace.space, "id", onDelete: .cascade))
            .field("type", informationTypeEnum, .required)
            .field("key", .custom("VARCHAR(64)"), .required)
            .field("text", .custom("VARCHAR(2048)"), .required)
            .field("text_short", .custom("VARCHAR(2048)"))
            .field("disruption_communication_id", .string)
            .field("disruption_id", .string)
            .create()
    }
    
    func revert(on database: any Database) async throws {
        // Tables
        try await database.schema(Information.schema, space: Information.space).delete()
        try await database.schema(ScheduleAtStopPlace.schema, space: ScheduleAtStopPlace.space).delete()
        try await database.schema(Transport.schema, space: Transport.space).delete()
        try await database.schema(Journey.schema, space: Journey.space).delete()
        try await database.schema(Administration.schema, space: Administration.space).delete()
        
        // Enums
        try await database.enum("InformationType").delete()
        try await database.enum("ScheduleType").delete()
        try await database.enum("JourneyType").delete()
    }
}
