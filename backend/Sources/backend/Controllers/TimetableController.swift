//
//  File.swift
//  backend
//
//  Created by Christian Knapp on 31.08.25.
//

import Vapor
import VaporToOpenAPI

enum TimetableProfile: String, LosslessStringConvertible, Codable, CaseIterable {
    case vendo, ris
    
    init?(_ description: String) {
        self.init(rawValue: description.lowercased())
    }
    
    var description: String {
        self.rawValue
    }
}

struct TimetableController: RouteCollection {
    func boot(routes: any RoutesBuilder) throws {
        let timetables = routes.grouped("timetable")
            .groupedOpenAPI(tags: ["Timetable"])
        
        timetables.grouped(":profile", "departures", ":evaNumber")
            .get(use: self.departures)
            .openAPI(
                summary: "Get departures",
                description: "Loads a set of departing journeys for a specific station.",
                query: .type(TimetableRequestDTO.QueryParams.self),
                path: .type(TimetableRequestDTO.PathParams.self)
            )
            .response(statusCode: .ok, body: .type([DepartureEntry].self), description: "Array of departure timetable entries")
            .response(statusCode: .badRequest, description: "Invalid parameters specified")
        
        timetables.grouped(":profile", "arrivals", ":evaNumber")
            .get(use: self.arrivals)
            .openAPI(
                summary: "Get arrivals",
                description: "Loads a set of arriving journeys for a specific station.",
                query: .type(TimetableRequestDTO.QueryParams.self),
                path: .type(TimetableRequestDTO.PathParams.self)
            )
            .response(statusCode: .ok, body: .type([ArrivalEntry].self), description: "Array of arrival timetable entries")
            .response(statusCode: .badRequest, description: "Invalid parameters specified")
    }

    func departures(req: Request) async throws -> [DepartureEntry] {
        let timetableRequest = try TimetableRequestDTO(req)
        return try await self.makeProvider(for: timetableRequest.profile).retrieveDepartures(for: timetableRequest, req: req)
    }
    
    func arrivals(req: Request) async throws -> [ArrivalEntry] {
        let timetableRequest = try TimetableRequestDTO(req)
        return try await self.makeProvider(for: timetableRequest.profile).retrieveArrivals(for: timetableRequest, req: req)
    }
    
    private func makeProvider(for profile: TimetableProfile) throws -> any TimetableProvider {
        switch profile {
        case .ris:
            return RisTimetableProvider()
        case .vendo:
            throw Abort(.notFound, reason: "This provider is not implemented yet")
        }
    }
}
