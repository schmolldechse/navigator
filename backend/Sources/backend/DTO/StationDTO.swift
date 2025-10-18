//
//  StationDTP.swift
//  backend
//
//  Created by Christian Knapp on 28.09.25.
//

import Vapor

struct StationDTO: Content {
    let evaNumber: Int
    let name: String
    let position: PositionDTO
    let ril100: [String]
    let transports: [TransportType]
    
    func toModel() -> Station {
        Station(
            id: self.evaNumber,
            name: self.name,
            weight: 0,
            latitude: self.position.latitude,
            longitude: self.position.longitude,
            queryingEnabled: false,
            lastQueried: nil,
            isLocked: false
        )
    }
}
