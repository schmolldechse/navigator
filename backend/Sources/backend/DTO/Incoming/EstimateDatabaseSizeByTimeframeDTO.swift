//
//  EstimateDatabaseSizeByTimeframeDTO.swift
//  backend
//
//  Created by Christian Knapp on 07.11.25.
//

import Vapor
import VaporToOpenAPI

struct EstimateDatabaseSizeByTimeframeDTO: Content, OpenAPIType, Decodable {
    let start: Date
    let end: Date
    
    enum CodingKeys: String, CodingKey {
        case start, end
    }
    
    init(from decoder: any Decoder) throws {
        let container = try decoder.container(keyedBy: CodingKeys.self)
                
        do {
            self.start = try container.decode(Date.self, forKey: .start)
        } catch {
            throw Abort(.badRequest, reason: "Invalid 'start' date format. Please use ISO 8601 format, e.g., yyyy-MM-dd'T'HH:mm:ssXXX.")
        }
            
        do {
            self.end = try container.decode(Date.self, forKey: .end)
        } catch {
            throw Abort(.badRequest, reason: "Invalid 'end' date format. Please use ISO 8601 format, e.g., yyyy-MM-dd'T'HH:mm:ssXXX.")
        }
    }
    
    private static var dateFormatter: DateFormatter {
        let formatter = DateFormatter()
        formatter.locale = Locale(identifier: "de_DE")
        formatter.timeZone = TimeZone(identifier: "Europe/Berlin")!
        formatter.dateFormat = "yyyy-MM-dd'T'HH:mm:ssXXX"
        return formatter
    }
    
    static var openAPISchema: SchemaObject {
        .object(properties: [
            "start": .dateTime()
                .with(\.description, "Starting date for the size estimation period")
                .with(\.default, AnyValue(Self.dateFormatter.string(from: Date().addingTimeInterval(-1*24*60*60))))
                .with(\.example, AnyValue(Self.dateFormatter.string(from: Date().addingTimeInterval(-1*24*60*60)))),
            "end": .dateTime()
                .with(\.description, "Ending date for the size estimation period")
                .with(\.default, AnyValue(Self.dateFormatter.string(from: Date())))
                .with(\.example, AnyValue(Self.dateFormatter.string(from: Date())))
        ])
    }
}
