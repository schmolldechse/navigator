//
//  StationByCoordinatesRequestDTO.swift
//  backend
//
//  Created by Christian Knapp on 25.10.25.
//

import Vapor
import VaporToOpenAPI

@OpenAPIDescriptable
struct StationByCoordinatesRequestDTO: Content {
    /// The longitude of the location.
    let latitude: Double
    /// The latitude of the location.
    let longitude: Double
    /// The maximum distance in meters to search for stations. Defaults to 1000.
    let maxDistanceMeters: Double?
    /// The maximum number of stations to return. Defaults to 100.
    let limit: Int?
}
