//
//  RisTimetableProvider.swift
//  backend
//
//  Created by Christian Knapp on 06.09.25.
//

import Vapor

struct RisTimetableProvider: TimetableProvider {
    private let apiUrl = "https://apis.deutschebahn.com/db/apis/ris-boards/v1/public/"
    
    func retrieveTimetable(for timetableRequest: TimetableRequestDTO, req: Request) async throws -> [TimetableEntryDTO] {
        guard let clientId = Environment.get("DB_CLIENT_ID"), let apiKey = Environment.get("DB_API_KEY") else {
            throw Abort(.internalServerError, reason: "Missing Deutsche Bahn API credentials.")
        }
        
        let response = try await req.client.get(URI(string: "\(apiUrl)/\(timetableRequest.type.rawValue)/\(timetableRequest.evaNumber)")) { clientReq in
            clientReq.headers.add(name: "DB-Client-Id", value: clientId)
            clientReq.headers.add(name: "DB-Api-Key", value: apiKey)
            clientReq.headers.add(name: "Accept", value: "application/vnd.de.db.ris+json")
        }
        
        guard response.status == .ok else {
            throw Abort(.internalServerError, reason: "Failed to fetch the timetable from RIS. Received: \(response.status.code) with '\(response.status.description)'.")
        }
        
        guard let responseBody = response.body else {
            throw Abort(.internalServerError, reason: "Failed to parse the response body from RIS.")
        }
        
        let data = Data(buffer: responseBody)
        guard let jsonObject = try? JSONSerialization.jsonObject(with: data) as? [String: Any] else {
            throw Abort(.internalServerError, reason: "Failed to parse JSON response into a dictionary.")
        }
        
        guard let events = jsonObject[timetableRequest.type.rawValue] as? [[String: Any]] else {
            throw Abort(.internalServerError, reason: "Could not find '\(timetableRequest.type.rawValue)' in the response.")
        }
        
        // Build timetable entries. Skip events that miss required identifiers for now.
        let timetableEntries: [TimetableEntryDTO] = events.compactMap { event in
            guard let journeyId = event["journeyID"] as? String else {
                return nil
            }
                        
            return TimetableEntryDTO(
                ris_journeyId: journeyId,
                hafas_journeyId: nil,
                origin: TimetableStopDTO(name: "goofy", evaNumber: 1),
                destination: TimetableStopDTO(name: "ssasd", evaNumber: 2),
                viaStops: [],
                timeInformation: TimeDTO(plannedTime: Date(), actualTime: Date(), delay: 0, plannedPlatform: nil, actualPlatform: nil),
                journeyInformation: TimetableJourneyDTO(productType: "type", productName: "name", journeyNumber: 123, journeyName: "journey", operatorName: "operator"),
                messages: [],
                cancelled: true
            )
        }
        
        return timetableEntries
    }
}
