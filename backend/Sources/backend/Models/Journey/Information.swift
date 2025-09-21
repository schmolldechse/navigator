//
//  Informaton.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent

final class Information: Model, @unchecked Sendable {
    static let schema: String = "journey_stop_place_informations"
    static let space: String? = "core"
    
    @ID(custom: "id", generatedBy: .database)
    var id: Int?
    
    @Parent(key: "scheduled_stop_place_id")
    var scheduledStopPlace: ScheduleAtStopPlace
    
    @Enum(key: "type")
    var type: InformationType
    
    @Field(key: "key")
    var key: String
    
    @Field(key: "text")
    var text: String
    
    @Field(key: "text_short")
    var textShort: String?
    
    @Field(key: "disruption_communication_id")
    var disruptionCommunicationID: String?
    
    @Field(key: "disruption_id")
    var disruptionID: String?
    
    init() { }
    
    init(id: Int? = nil, scheduledStopPlaceID: ScheduleAtStopPlace.IDValue, type: InformationType, key: String, text: String, textShort: String? = nil, disruptionCommunicationID: String? = nil, disruptionID: String? = nil) {
        self.id = id
        self.$scheduledStopPlace.id = scheduledStopPlaceID
        self.type = type
        self.key = key
        self.text = text
        self.textShort = textShort
        self.disruptionCommunicationID = disruptionCommunicationID
        self.disruptionID = disruptionID
    }
}
