// 
// PositionDTO.swift
// backend
//
// Created by Christian Knapp on 17.10.25.
//

import Vapor 

struct PositionDTO: Content {
    let latitude: Double
    let longitude: Double
}
