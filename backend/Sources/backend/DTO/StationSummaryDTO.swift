//
//  StationSummaryDTO.swift
//  backend
//
//  Created by Christian Knapp on 25.10.25.
//

import Vapor

struct StationSummaryDTO: Content {
    let evaNumber: Int
    let name: String
    let position: PositionDTO
}
