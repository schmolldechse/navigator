//
//  StationByEvaNumberRequestDTO.swift
//  backend
//
//  Created by Christian Knapp on 18.10.25.
//

import Vapor
import VaporToOpenAPI

@OpenAPIDescriptable
struct StationByEvaNumberRequestDTO: Content {
    /// The EVA number of the station.
    let evaNumber: Int
}
