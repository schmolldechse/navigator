// 
// StationController.swift
// backend
//
// Created by Christian Knapp on 17.10.25.
//

import Vapor

struct StationController: RouteCollection {
    func boot(routes: any RoutesBuilder) throws {
        let stations = routes.grouped("stations")
            .groupedOpenAPI(tags: ["Stations"])

        stations.get(use: queryStations)
            .openAPI(
                summary: "Query stations",
                description: "Loads a set of stations matching the search criteria.",
                query: .type(VendoStationSearchRequestDTO.self)
            )
            .response(statusCode: .ok, body: .type([StationDTO].self), description: "Array of stations matching the search criteria")
            .response(statusCode: .badGateway, description: "Failed to query stations from Vendo.")
            .response(statusCode: .internalServerError, description: "Failed to parse the response body.")
        
        stations.get(":evaNumber", use: getStationByEvaNumber)
            .openAPI(
                summary: "Get station by EVA number",
                description: "Loads the station for a specific EVA number.",
                path: .type(StationByEvaNumberRequestDTO.self)
            )
            .response(statusCode: .ok, body: .type(StationDTO.self), description: "Station matching the specified EVA number")
            .response(statusCode: .notFound, description: "No station found for the specified EVA number.")
            .response(statusCode: .badGateway, description: "Failed to query stations from Vendo.")
            .response(statusCode: .internalServerError, description: "Failed to parse the response body.")
    }

    private func queryStations(req: Request) async throws -> [StationDTO] {
        let searchRequest = try req.query.decode(VendoStationSearchRequestDTO.self)
        return try await req.stationRepository.query(searchTerm: searchRequest.searchTerm)
    }
    
    private func getStationByEvaNumber(req: Request) async throws -> StationDTO {
        guard let evaNumber = req.parameters.get("evaNumber", as: Int.self) else {
            throw Abort(.badRequest, reason: "Invalid 'evaNumber' specified. It must be an integer.")
        }
        return try await req.stationRepository.findByEvaNumber(evaNumber: evaNumber)
    }
}
