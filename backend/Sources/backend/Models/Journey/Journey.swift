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
    
    @Field(key: "inserted_at")
    var insertedAt: Date
    
    @Field(key: "type")
    var type: JourneyType
    
    @Parent(key: "administration_index")
    var administration: Administration
    
    @Field(key: "cancelled")
    var cancelled: Bool
    
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
