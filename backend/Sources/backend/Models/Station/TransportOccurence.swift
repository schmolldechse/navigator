//
//  ProductOccurence.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent

final class TransportOccurence: Model, @unchecked Sendable {
    static let schema: String = "station_transports"
    static let space: String? = "core"
    
    @ID(custom: "id", generatedBy: .database)
    var id: Int?
    
    @Enum(key: "transport_name")
    var transport: TransportType
    
    @Field(key: "querying_enabled")
    var queryingEnabled: Bool
    
    @Parent(key: "eva_number")
    var station: Station
    
    init() { }
    
    init(id: Int? = nil, transport: TransportType, queryingEnabled: Bool, evaNumber: Station.IDValue) {
        self.id = id
        self.transport = transport
        self.queryingEnabled = queryingEnabled
        self.$station.id = evaNumber
    }
}
