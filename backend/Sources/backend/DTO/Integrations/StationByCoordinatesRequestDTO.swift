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
    let maxDistanceMeters: Double
    /// The maximum number of stations to return. Defaults to 100.
    let limit: Int
    
    enum CodingKeys: String, CodingKey {
        case latitude
        case longitude
        case maxDistanceMeters
        case limit
    }
    
    init(from decoder: any Decoder) throws {
        let container = try decoder.container(keyedBy: CodingKeys.self)
        self.latitude = try container.decode(Double.self, forKey: .latitude)
        self.longitude = try container.decode(Double.self, forKey: .longitude)
        self.maxDistanceMeters = try container.decodeIfPresent(Double.self, forKey: .maxDistanceMeters) ?? 1000.0
        self.limit = try container.decodeIfPresent(Int.self, forKey: .limit) ?? 100
    }
    
    init(latitude: Double, longitude: Double, maxDistanceMeters: Double = 1000.0, limit: Int = 100) {
        self.latitude = latitude
        self.longitude = longitude
        self.maxDistanceMeters = maxDistanceMeters
        self.limit = limit
    }
}
