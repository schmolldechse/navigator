//
//  Ril100.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent

final class Ril100: Model, @unchecked Sendable {
    static let schema: String = "station_ril100"
    static let space: String? = "core"
    
    @ID(custom: "id", generatedBy: .database)
    var id: Int?
    
    @Field(key: "ril100")
    var ril100: String
    
    @Parent(key: "eva_number")
    var station: Station
    
    init() { }
    
    init(id: Int? = nil, ril100: String, evaNumber: Station.IDValue) {
        self.id = id
        self.ril100 = ril100
        self.$station.id = evaNumber
    }
}
