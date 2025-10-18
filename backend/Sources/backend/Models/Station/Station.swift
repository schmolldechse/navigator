//
//  Station.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent
import Foundation

final class Station: Model, @unchecked Sendable {
    static let schema: String = "stations"
    static let space: String? = "core"
    
    @ID(custom: "eva_number", generatedBy: .user)
    var id: Int?
    
    @Field(key: "name")
    var name: String
    
    @Field(key: "weight")
    var weight: Double
    
    @Field(key: "latitude")
    var latitude: Double
    
    @Field(key: "longitude")
    var longitude: Double
    
    @Field(key: "querying_enabled")
    var queryingEnabled: Bool
    
    @Field(key: "last_queried")
    var lastQueried: Date?
    
    @Field(key: "is_locked")
    var isLocked: Bool
    
    @Children(for: \.$station)
    var transportOccurences: [TransportOccurence]
    
    @Children(for: \.$station)
    var ril100: [Ril100]

    init() { }
    
    init(id: Int, name: String, weight: Double = 0, latitude: Double, longitude: Double, queryingEnabled: Bool = false, lastQueried: Date? = nil, isLocked: Bool = false) {
        self.id = id
        self.name = name
        self.weight = weight
        self.latitude = latitude
        self.longitude = longitude
        self.queryingEnabled = queryingEnabled
        self.lastQueried = lastQueried
        self.isLocked = isLocked
    }
}
