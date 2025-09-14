//
//  File.swift
//  backend
//
//  Created by Christian Knapp on 02.09.25.
//

import Vapor
import SwiftOpenAPI

struct TimetableRequestDTO: Content {
    
    // Path Parameters
    let profile: TimetableProfile
    let evaNumber: Int
    
    // Query Parameters
    var when: Date?
    var duration: Int?
    
    init(_ request: Request) throws {
        guard let profile = request.parameters.get("profile", as: TimetableProfile.self) else {
            throw Abort(.badRequest, reason: "Invalid 'profile' specified. Valid profiles are: \(TimetableProfile.allCases.map(\.rawValue).joined(separator: ", "))")
        }
        
        guard let evaNumber = request.parameters.get("evaNumber", as: Int.self) else {
            throw Abort(.badRequest, reason: "Invalid 'evaNumber' specified. It must be an integer.")
        }
        
        let query = try request.query.decode(QueryParams.self)
        
        self.profile = profile
        self.evaNumber = evaNumber
        
        self.when = query.when ?? Date()
        self.duration = query.duration ?? 60
    }
    
    @OpenAPIDescriptable
    struct PathParams: Decodable {
        /// The profile to use.
        let profile: TimetableProfile
        /// The station to load the timetable for.
        let evaNumber: Int
    }
    
    @OpenAPIDescriptable
    struct QueryParams: Content, Validatable {
        /// When to look for a timetable. Defaults to the current time if not provided.
        let when: Date?
        /// The duration in minutes. Defaults to 60. Must be 1 or greater if provided.
        let duration: Int?
        
        static func validations(_ validations: inout Validations) {
            validations.add("duration", as: Int.self, is: .range(1...), required: false)
        }
    }
}
