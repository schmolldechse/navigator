//
//  Journey.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent
import Foundation

final class Journey: Model, @unchecked Sendable {
    static let schema: String = "journeys"
    static let space: String? = "core"
    
    @ID(custom: "journey_id", generatedBy: .user)
    var id: String?
    
    @Field(key: "date")
    var date: Date
    
    @Timestamp(key: "inserted_at", on: .create)
    var insertedAt: Date?
    
    @Field(key: "type")
    var type: JourneyType
    
    @Parent(key: "administration_index")
    var administration: Administration
    
    @OptionalChild(for: \.$journey)
    var transport: Transport?
    
    @Children(for: \.$journey)
    var viaStops: [ScheduleAtStopPlace]
    
    init() { }
    
    init(journeyId: String, date: Date, type: JourneyType, administrationIndex: Administration.IDValue) {
        self.id = journeyId
        self.date = date
        self.type = type
        self.$administration.id = administrationIndex
    }
}
