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

enum TimetableType: String, LosslessStringConvertible, Codable, CaseIterable {
    case arrivals, departures

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

        timetables.group(":profile", ":evaNumber", ":type") { timetable in
            timetable.get(use: self.index)
                .openAPI(
                    summary: "Load timetable",
                    description: "Loads a timetable for a specific station",
                    query: .type(TimetableRequestDTO.QueryParams.self),
                    path: .type(TimetableRequestDTO.PathParams.self)
                )
                .response(statusCode: .ok, body: .type([TimetableEntryDTO].self), description: "Array of timetable entries")
                .response(statusCode: .badRequest, description: "Invalid parameters specified")
        }
    }

    func index(req: Request) async throws -> [TimetableEntryDTO] {
        let timetableRequest = try TimetableRequestDTO(req)
        return try await self.makeProvider(for: timetableRequest.profile).retrieveTimetable(for: timetableRequest, req: req)
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
