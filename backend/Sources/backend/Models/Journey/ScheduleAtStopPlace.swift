//
//  ScheduleAtStopPlace.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent
import Foundation

internal enum ScheduleType: String, Codable, CaseIterable {
    case arrival = "ARRIVAL"
    case departure = "DEPARTURE"
}

final class ScheduleAtStopPlace: Model, @unchecked Sendable {
    static let schema: String = "journey_scheduled_stop_places"
    static let space: String? = "core"
    
    @ID(custom: "id", generatedBy: .database)
    var id: Int?
    
    @Parent(key: "journey_id")
    var journey: Journey
    
    @Field(key: "date")
    var date: Date
    
    @Enum(key: "type")
    var type: ScheduleType
    
    @Field(key: "station_name")
    var stationName: String
    
    @Field(key: "station_eva_number")
    var stationEvaNumber: Int
    
    @Field(key: "cancelled")
    var cancelled: Bool
    
    @Field(key: "additional")
    var additional: Bool
    
    @Field(key: "demand")
    var demand: Bool
    
    @Field(key: "no_passenger_change")
    var noPassengerChange: Bool
    
    @Field(key: "planned_time")
    var plannedTime: Date
    
    @Field(key: "actual_time")
    var actualTime: Date
    
    @Field(key: "delay")
    var delay: Int
    
    @Field(key: "planned_platform")
    var plannedPlatform: String?
    
    @Field(key: "actual_platform")
    var actualPlatform: String?
    
    @Children(for: \.$scheduledStopPlace)
    var information: [Information]
    
    init() { }
    
    init(id: Int? = nil, journey: Journey, date: Date, type: ScheduleType, stationName: String, stationEvaNumber: Int, cancelled: Bool, additional: Bool, demand: Bool, noPassengerChange: Bool, plannedTime: Date, actualTime: Date, delay: Int, plannedPlatform: String? = nil, actualPlatform: String? = nil) {
        self.id = id
        self.journey = journey
        self.date = date
        self.type = type
        self.stationName = stationName
        self.stationEvaNumber = stationEvaNumber
        self.cancelled = cancelled
        self.additional = additional
        self.demand = demand
        self.noPassengerChange = noPassengerChange
        self.plannedTime = plannedTime
        self.actualTime = actualTime
        self.delay = delay
        self.plannedPlatform = plannedPlatform
        self.actualPlatform = actualPlatform
    }
}
