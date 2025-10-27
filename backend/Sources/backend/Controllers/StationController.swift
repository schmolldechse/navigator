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
                summary: "Search stations by name",
                description: "Performs a fuzzy search for stations based on the provided search term. This endpoint queries the Deutsche Bahn API to find and return a list of stations that best match the search criteria.",
                query: .type(VendoStationSearchRequestDTO.self)
            )
            .response(statusCode: .ok, body: .type([StationDetailDTO].self), description: "Array of stations matching the search criteria.")
            .response(statusCode: .badGateway, description: "Failed to query stations from Vendo.")
            .response(statusCode: .internalServerError, description: "Failed to parse the response body.")
        
        stations.get(":evaNumber", use: getStationByEvaNumber)
            .openAPI(
                summary: "Get station by EVA number",
                description: "Retrieves detailed information for a single station using its unique `evaNumber`. This is an exact lookup and will only return a station if the `evaNumber` matches perfectly.",
                path: .type(StationByEvaNumberRequestDTO.self)
            )
            .response(statusCode: .ok, body: .type(StationDetailDTO.self), description: "Station matching the specified EVA number.")
            .response(statusCode: .notFound, description: "No station found for the specified EVA number.")
            .response(statusCode: .badGateway, description: "Failed to query stations from Vendo.")
            .response(statusCode: .internalServerError, description: "Failed to parse the response body.")
        
        stations.post("nearby", use: getStationByCoordinates)
            .openAPI(
                summary: "Find stations by coordinates",
                description: "Finds all stations within a specified radius of a central coordinate.",
                body: .type(StationByCoordinatesRequestDTO.self)
            )
            .response(statusCode: .ok, body: .type([StationSummaryDTO].self), description: "Array of stations near the specified coordinates.")
            .response(statusCode: .internalServerError, description: "An error occured while processing the request.")
        
        stations.get("gathering", ":evaNumber", use: getStationGatheringInfo)
            .openAPI(
                summary: "Check station gathering info",
                description: "Retrieves gathering information for a specific station identified by its `evaNumber`.",
                path: .type(StationByEvaNumberRequestDTO.self)
            )
            .response(statusCode: .ok, body: .type(StationGatheringInfoDTO.self), description: "Gathering information for the specified station.")
            .response(statusCode: .notFound, description: "No station found for the specified EVA number.")
    }

    private func queryStations(req: Request) async throws -> [StationDetailDTO] {
        let searchRequest = try req.query.decode(VendoStationSearchRequestDTO.self)
        return try await req.stationRepository.queryFor(searchTerm: searchRequest.searchTerm)
    }
    
    private func getStationByEvaNumber(req: Request) async throws -> StationDetailDTO {
        guard let evaNumber = req.parameters.get("evaNumber", as: Int.self) else {
            throw Abort(.badRequest, reason: "Invalid 'evaNumber' specified. It must be an integer.")
        }
        return try await req.stationRepository.findBy(evaNumber: evaNumber)
    }
    
    private func getStationByCoordinates(req: Request) async throws -> [StationSummaryDTO] {
        let coordinateRequest = try req.content.decode(StationByCoordinatesRequestDTO.self)
        return try await req.stationRepository.findBy(request: coordinateRequest)
    }
    
    private func getStationGatheringInfo(req: Request) async throws -> StationGatheringInfoDTO {
        guard let evaNumber = req.parameters.get("evaNumber", as: Int.self) else {
            throw Abort(.badRequest, reason: "Invalid 'evaNumber' specified. It must be an integer.")
        }
        return try await req.stationRepository.getGatheringInfo(evaNumber: evaNumber)
    }
}

