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
}
