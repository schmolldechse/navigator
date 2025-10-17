//
// VendoStationSearchRequestDTO.swift
// backend
//
// Created by Christian Knapp on 17.10.25.
//

import Vapor

struct VendoStationSearchRequestDTO: Content {
    let searchTerm: String
    let maxResults: Int = 10
    let locationTypes: [String] = ["ALL"]
}