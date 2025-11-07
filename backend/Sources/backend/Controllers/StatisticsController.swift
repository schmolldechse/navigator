//
//  StatisticsController.swift
//  backend
//
//  Created by Christian Knapp on 07.11.25.
//

import Vapor

struct StatisticsController: RouteCollection {
    func boot(routes: any RoutesBuilder) throws {
        let stations = routes.grouped("statistics")
            .groupedOpenAPI(tags: ["Statistics"])

        stations.post("size", "estimate", use: estimateSize)
            .openAPI(
                summary: "Estimate the Database size",
                description: "Estimates the current size of the database.",
                body: .type(EstimateDatabaseSizeByTimeframeDTO.self)
            )
        .response(statusCode: .ok, body: .type([MeasuredTimeframeStatisticDTO].self), description: "Estimated database size over time.")
        .response(statusCode: .internalServerError, description: "Failed to estimate database size.")
    }

    private func estimateSize(req: Request) async throws -> MeasuredTimeframeStatisticDTO {
        let sizeRequest = try req.content.decode(EstimateDatabaseSizeByTimeframeDTO.self)
        return try await req.application.statisticsRepository.estimateSize(request: sizeRequest)
    }
}
