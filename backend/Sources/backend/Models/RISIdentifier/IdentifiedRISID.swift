//
//  IdentifiedRISID.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent
import Foundation

final class IdentifiedRISID: Model, @unchecked Sendable {
    static let schema: String = "ris_ids"
    static let space: String? = "core"
    
    @ID(custom: "id", generatedBy: .user)
    var id: String?
    
    @Enum(key: "transport_type")
    var transportType: TransportType
    
    @OptionalEnum(key: "replacement_transport_type")
    var replacementTransportType: TransportType?
    
    @Field(key: "discovery_date")
    var discoveryDate: Date
    
    @Field(key: "last_seen")
    var lastSeen: Date?
    
    @Field(key: "last_succeeded_at")
    var lastSucceededAt: Date?
    
    @Field(key: "active")
    var active: Bool
    
    @Field(key: "is_locked")
    var isLocked: Bool
    
    init() { }
    
    init(id: String, transportType: TransportType, replacementTransportType: TransportType? = nil, discoveryDate: Date = Date(), lastSeen: Date? = nil, lastSucceededAt: Date? = nil, active: Bool = true, isLocked: Bool = false) {
        self.id = id
        self.transportType = transportType
        self.replacementTransportType = replacementTransportType
        self.discoveryDate = discoveryDate
        self.lastSeen = lastSeen
        self.lastSucceededAt = lastSucceededAt
        self.active = active
        self.isLocked = isLocked
    }
}

