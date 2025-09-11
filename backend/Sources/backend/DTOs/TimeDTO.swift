//
//  File.swift
//  backend
//
//  Created by Christian Knapp on 11.09.25.
//

import Vapor
import SwiftOpenAPI

struct TimeDTO: Content, OpenAPIDescriptable {
    var plannedTime: Date
    var actualTime: Date
    var delay: Int
    
    var plannedPlatform: String?
    var actualPlatform: String?
    
    static var openAPIDescription: (any OpenAPIDescriptionType)? {
        OpenAPIDescription<CodingKeys>()
            .add(for: .plannedTime, "")
    }
}
