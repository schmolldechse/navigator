//
// VendoStationSearchRequestDTO.swift
// backend
//
// Created by Christian Knapp on 17.10.25.
//

import Vapor
import VaporToOpenAPI

@OpenAPIDescriptable
struct VendoStationSearchRequestDTO: Content {
    /// The search term to look for stations.
    let searchTerm: String
    /// The maximum number of results to return. Default is 10.
    let maxResults: Int
    /// The types of locations to include in the search. Default is ["ALL"].
    let locationTypes: [String]
    
    enum CodingKeys: CodingKey {
        case searchTerm
        case maxResults
        case locationTypes
    }
    
    init(from decoder: any Decoder) throws {
        let container = try decoder.container(keyedBy: CodingKeys.self)
        self.searchTerm = try container.decode(String.self, forKey: .searchTerm)
        self.maxResults = try container.decodeIfPresent(Int.self, forKey: .maxResults) ?? 10
        self.locationTypes = try container.decodeIfPresent([String].self, forKey: .locationTypes) ?? ["ALL"]
    }
    
    init(searchTerm: String, maxResults: Int = 10, locationTypes: [String] = ["ALL"]) {
        self.searchTerm = searchTerm
        self.maxResults = maxResults
        self.locationTypes = locationTypes
    }
}
