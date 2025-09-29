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

@OpenAPIDescriptable
struct TimetablePathParams: Decodable {
    /// The profile to use.
    let profile: TimetableProfile
    /// The station to load the timetable for.
    let evaNumber: Int
}

@OpenAPIDescriptable
struct TimetableQueryParams: Content, Validatable {
    /// When to look for a timetable. Defaults to the current time if not provided.
    var when: Date?
    /// The duration in minutes. Defaults to 60. Must be 1 or greater if provided.
    var duration: Int?
    
    static func validations(_ validations: inout Validations) {
        validations.add("duration", as: Int.self, is: .range(1...), required: false)
    }
    
    init(from decoder: any Decoder) throws {
        enum CodingKeys: String, CodingKey {
            case when, duration
        }
        
        let container = try decoder.container(keyedBy: CodingKeys.self)
                    
        self.duration = try container.decodeIfPresent(Int.self, forKey: .duration)
                    
        if let dateString = try container.decodeIfPresent(String.self, forKey: .when) {
            let formatter = ISO8601DateFormatter()
            formatter.formatOptions = [.withInternetDateTime, .withColonSeparatorInTimeZone]
                        
            if let date = formatter.date(from: dateString) {
                self.when = date
            } else {
                throw DecodingError.dataCorruptedError(forKey: .when, in: container, debugDescription: "Date string '\(dateString)' does not match expected ISO 8601 format.")
            }
        } else {
            self.when = nil
        }
    }
}

struct TimetableRequest {
    let profile: TimetableProfile
    let evaNumber: Int
    var when: Date
    var duration: Int
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
                query: .type(TimetableQueryParams.self),
                path: .type(TimetablePathParams.self)
            )
            .response(statusCode: .ok, body: .type([DepartureEntry].self), description: "Array of departure timetable entries")
            .response(statusCode: .badRequest, description: "Invalid parameters specified")
        
        timetables.grouped(":profile", "arrivals", ":evaNumber")
            .get(use: self.arrivals)
            .openAPI(
                summary: "Get arrivals",
                description: "Loads a set of arriving journeys for a specific station.",
                query: .type(TimetableQueryParams.self),
                path: .type(TimetablePathParams.self)
            )
            .response(statusCode: .ok, body: .type([ArrivalEntry].self), description: "Array of arrival timetable entries")
            .response(statusCode: .badRequest, description: "Invalid parameters specified")
    }

    func departures(req: Request) async throws -> [DepartureEntry] {
        let timetableRequest = try self.makeTimetableRequest(for: req)
        return try await self.makeProvider(for: timetableRequest.profile).retrieveDepartures(for: timetableRequest, req: req)
    }
    
    func arrivals(req: Request) async throws -> [ArrivalEntry] {
        let timetableRequest = try self.makeTimetableRequest(for: req)
        return try await self.makeProvider(for: timetableRequest.profile).retrieveArrivals(for: timetableRequest, req: req)
    }
    
    private func makeTimetableRequest(for request: Request) throws -> TimetableRequest {
        guard let profile = request.parameters.get("profile", as: TimetableProfile.self) else {
            throw Abort(.badRequest, reason: "Invalid 'profile' specified. Valid profiles are: \(TimetableProfile.allCases.map(\.rawValue).joined(separator: ", "))")
        }
        
        guard let evaNumber = request.parameters.get("evaNumber", as: Int.self) else {
            throw Abort(.badRequest, reason: "Invalid 'evaNumber' specified. It must be an integer.")
        }
        
        var timetableRequest = TimetableRequest(profile: profile, evaNumber: evaNumber, when: Date(), duration: 60)
        
        let query = try request.query.decode(TimetableQueryParams.self)
        timetableRequest.when = query.when ?? Date()
        timetableRequest.duration = query.duration ?? 60
        return timetableRequest
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
