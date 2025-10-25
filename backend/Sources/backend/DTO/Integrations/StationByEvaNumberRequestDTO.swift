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
    
    enum CodingKeys: String, CodingKey {
        case evaNumber
    }
    
    init(from decoder: any Decoder) throws {
        let container = try decoder.container(keyedBy: CodingKeys.self)
        self.evaNumber = try container.decode(Int.self, forKey: .evaNumber)
    }
    
    init(evaNumber: Int) {
        self.evaNumber = evaNumber
    }
}
