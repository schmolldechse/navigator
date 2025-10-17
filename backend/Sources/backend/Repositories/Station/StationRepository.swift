//
// StationRepository.swift
// backend
//
// Created by Christian Knapp on 17.10.25.
//

import Vapor

protocol StationRepository {
    func query(searchTerm: String) async throws -> [StationDTO]
    func findByEvaNumber(evaNumber: Int, on req: Request) async throws -> StationDTO?
}
