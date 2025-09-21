//
//  Transport.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent

final class Transport: Model, @unchecked Sendable {
    static let schema: String = "journey_transports"
    static let space: String? = "core"
    
    @ID(custom: "journey_id", generatedBy: .user)
    var id: String?
    
    @Parent(key: "journey_id")
    var journey: Journey
    
    @Enum(key: "type")
    var type: TransportType
    
    @OptionalEnum(key: "replacement_type")
    var replacementType: TransportType?
    
    @Field(key: "category")
    var category: String
    
    @Field(key: "category_internal")
    var categoryInternal: String
    
    @Field(key: "journey_description")
    var journeyDescription: String
    
    @Field(key: "label")
    var label: String
    
    @Field(key: "number")
    var number: Int
    
    @OptionalField(key: "line")
    var line: String?

    init() { }
    
    init(journeyId: Journey.IDValue, type: TransportType, replacementType: TransportType? = nil, category: String, categoryInternal: String, journeyDescription: String, label: String, number: Int, line: String? = nil) {
        self.id = journeyId
        self.$journey.id = journeyId
        self.type = type
        self.replacementType = replacementType
        self.category = category
        self.categoryInternal = categoryInternal
        self.journeyDescription = journeyDescription
        self.label = label
        self.number = number
        self.line = line
    }
}
