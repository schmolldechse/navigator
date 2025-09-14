//
//  RisTimetableProvider.swift
//  backend
//
//  Created by Christian Knapp on 06.09.25.
//

import Vapor

struct RisTimetableProvider: TimetableProvider {
    private let apiUrl = "https://apis.deutschebahn.com/db/apis/ris-boards/v1/public/"
    private let journeyPattern = "\\s\\(.*\\)"
    
    private func requestTimetable(for timetableRequest: TimetableRequestDTO, isDeparture: Bool, req: Request) async throws -> [String: Any] {
        guard let clientId = Environment.get("DB_CLIENT_ID"), let apiKey = Environment.get("DB_API_KEY") else {
            throw Abort(.internalServerError, reason: "Missing Deutsche Bahn API credentials.")
        }
                
        let endTime: Date = timetableRequest.when!.addingTimeInterval(Double(timetableRequest.duration!) * 60.0)
        let response = try await req.client.get(URI(string: "\(apiUrl)/\(isDeparture ? "departures" : "arrivals")/\(timetableRequest.evaNumber)?timeEnd=\(endTime.ISO8601Format())&timeStart=\(timetableRequest.when!.ISO8601Format())")) { clientReq in
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
        return jsonObject
    }
    
    func retrieveDepartures(for timetableRequest: TimetableRequestDTO, req: Request) async throws -> [DepartureEntry] {
        let timetable = try await self.requestTimetable(for: timetableRequest, isDeparture: true, req: req)
        guard let boards = timetable["departures"] as? [[String: Any]] else {
            throw Abort(.internalServerError, reason: "Could not find 'departures' in the response.")
        }
        
        return boards.compactMap { boardEntry -> DepartureEntry? in
            guard
                let journeyId = boardEntry["journeyID"] as? String,
                let transportEntry = boardEntry["transport"] as? [String: Any]
            else {
                return nil
            }
            
            let destination: StopAtStopPlace = {
                let destinationObj = transportEntry["destination"] as! [String: Any]
                return StopAtStopPlace(
                    name: destinationObj["name"] as! String,
                    evaNumber: Int(destinationObj["evaNumber"] as! String)!,
                    cancelled: destinationObj["canceled"] as! Bool
                )
            }()
            
            let direction: [StopPlace] = {
                guard let directionObj = transportEntry["direction"] as? [String: Any] else { return [] }
                return (directionObj["stopPlaces"] as? [[String: Any]] ?? []).compactMap { stopPlaceObj -> StopPlace in
                    StopPlace(name: stopPlaceObj["name"] as! String, evaNumber: Int(stopPlaceObj["evaNumber"] as! String)!)
                }
            }()
            
            return DepartureEntry(
                ris_journeyId: journeyId,
                administration: self.buildAdministration(for: boardEntry),
                transport: self.buildTransport(for: transportEntry, type: boardEntry["journeyType"] as! String),
                destination: destination,
                differingDestination: transportEntry["differingDestination"] as? [String: Any] != nil ? StopAtStopPlace(
                    name: (transportEntry["differingDestination"] as! [String: Any])["name"] as! String,
                    evaNumber: Int((transportEntry["differingDestination"] as! [String: Any])["evaNumber"] as! String)!,
                    cancelled: (transportEntry["differingDestination"] as! [String: Any])["canceled"] as! Bool,
                    additional: (transportEntry["differingDestination"] as! [String: Any])["additional"] as? Bool
                ) : nil,
                direction: direction,
                viaStops: (transportEntry["via"] as? [[String: Any]] ?? []).compactMap { stopPlaceObj -> StopAtStopPlace in
                    StopAtStopPlace(
                        name: stopPlaceObj["name"] as! String,
                        evaNumber: Int(stopPlaceObj["evaNumber"] as! String)!,
                        cancelled: stopPlaceObj["canceled"] as! Bool,
                        additional: stopPlaceObj["additional"] as? Bool
                    )
                },
                departure: self.buildSchedule(for: boardEntry),
                informations: self.buildInformations(for: boardEntry),
                cancelled: boardEntry["canceled"] as! Bool,
                additional: boardEntry["additional"] as? Bool,
                demand: boardEntry["onDemand"] as? Bool,
                travelsWith: (boardEntry["travelsWith"] as? [[String: Any]] ?? []).compactMap { travelObj -> CoupledTransport? in
                    guard let separationAt = travelObj["separationAt"] as? [String: Any] else { return nil }
                    return CoupledTransport(
                        journeyId: travelObj["journeyID"] as! String,
                        separationAt: StopPlace(
                            name: separationAt["name"] as! String,
                            evaNumber: Int(separationAt["evaNumber"] as! String)!
                        )
                    )
                }
            )
        }
    }
    
    func retrieveArrivals(for timetableRequest: TimetableRequestDTO, req: Request) async throws -> [ArrivalEntry] {
        let timetable = try await self.requestTimetable(for: timetableRequest, isDeparture: false, req: req)
        guard let boards = timetable["arrivals"] as? [[String: Any]] else {
            throw Abort(.internalServerError, reason: "Could not find 'arrivals' in the response.")
        }
        
        return boards.compactMap { boardEntry -> ArrivalEntry? in
            guard
                let journeyId = boardEntry["journeyID"] as? String,
                let transportEntry = boardEntry["transport"] as? [String: Any]
            else {
                return nil
            }
            
            let origin: StopAtStopPlace = {
                let originObj = transportEntry["origin"] as! [String: Any]
                return StopAtStopPlace(
                    name: originObj["name"] as! String,
                    evaNumber: Int(originObj["evaNumber"] as! String)!,
                    cancelled: originObj["canceled"] as! Bool
                )
            }()
            
            let direction: [StopPlace] = {
                guard let directionObj = transportEntry["direction"] as? [String: Any] else { return [] }
                return (directionObj["stopPlaces"] as? [[String: Any]] ?? []).compactMap { stopPlaceObj -> StopPlace in
                    StopPlace(name: stopPlaceObj["name"] as! String, evaNumber: Int(stopPlaceObj["evaNumber"] as! String)!)
                }
            }()
            
            return ArrivalEntry(
                ris_journeyId: journeyId,
                administration: self.buildAdministration(for: boardEntry),
                transport: self.buildTransport(for: transportEntry, type: boardEntry["journeyType"] as! String),
                origin: origin,
                differingOrigin: transportEntry["differingOrigin"] as? [String: Any] != nil ? StopAtStopPlace(
                    name: (transportEntry["differingOrigin"] as! [String: Any])["name"] as! String,
                    evaNumber: Int((transportEntry["differingOrigin"] as! [String: Any])["evaNumber"] as! String)!,
                    cancelled: (transportEntry["differingOrigin"] as! [String: Any])["canceled"] as! Bool,
                    additional: (transportEntry["differingOrigin"] as! [String: Any])["additional"] as? Bool
                ) : nil,
                direction: direction,
                viaStops: (transportEntry["via"] as? [[String: Any]] ?? []).compactMap { stopPlaceObj -> StopAtStopPlace in
                    StopAtStopPlace(
                        name: stopPlaceObj["name"] as! String,
                        evaNumber: Int(stopPlaceObj["evaNumber"] as! String)!,
                        cancelled: stopPlaceObj["canceled"] as! Bool,
                        additional: stopPlaceObj["additional"] as? Bool
                    )
                },
                arrival: self.buildSchedule(for: boardEntry),
                informations: self.buildInformations(for: boardEntry),
                cancelled: boardEntry["canceled"] as! Bool,
                additional: boardEntry["additional"] as? Bool,
                demand: boardEntry["onDemand"] as? Bool,
                travelsWith: (boardEntry["travelsWith"] as? [[String: Any]] ?? []).compactMap { travelObj -> String? in
                    guard let journeyId = travelObj["journeyID"] as? String else { return nil }
                    return journeyId
                }
            )
        }
    }
    
    private func buildSchedule(for timetableEntry: [String: Any]) -> ScheduleAtStopPlace {
        let plannedTime: Date = ((timetableEntry["timeSchedule"] as? String)?.toDate())!
        let actualTime: Date = ((timetableEntry["time"] as? String)?.toDate())!
        let delay: Int = Int(actualTime.timeIntervalSince(plannedTime))
        
        return ScheduleAtStopPlace(
            plannedTime: plannedTime,
            actualTime: actualTime,
            delay: delay,
            plannedPlatform: timetableEntry["platformSchedule"] as? String,
            actualPlatform: timetableEntry["platform"] as? String
        )
    }
    
    private func buildTransport(for transportEntry: [String: Any], type: String) -> Transport {
        let replacementType: TransportType? = {
            guard let replacementTypeObj = transportEntry["replacementTransport"] as? [String: Any] else { return nil }
            return TransportType(rawValue: (replacementTypeObj["realType"] as! String).uppercased()) ?? .UNKNOWN
        }()
        
        let journeyDescription: String = {
            guard let line = transportEntry["line"] as? String else { return self.simplifyDescription(for: transportEntry["journeyDescription"] as! String) }
            return (transportEntry["category"] as! String) + " " + line
        }()
                
        return Transport(
            type: TransportType(rawValue: (transportEntry["type"] as! String).uppercased()) ?? .UNKNOWN,
            replacementType: replacementType,
            category: transportEntry["category"] as! String,
            journeyType: JourneyType(rawValue: type.uppercased())!,
            journeyDescription: journeyDescription,
            number: transportEntry["number"] as! Int,
            line: transportEntry["line"] as? String
        )
    }
    
    private func buildAdministration(for timetableEntry: [String: Any]) -> Administration? {
        guard let administration = timetableEntry["administration"] as? [String: Any] else {
            return nil
        }
        
        return Administration(
            administrationId: administration["administrationID"] as! String,
            operatorCode: administration["operatorCode"] as! String,
            operatorName: administration["operatorName"] as! String
        )
    }
    
    private func buildInformations(for timetableEntry: [String: Any]) -> [Information] {
        var informations: [Information] = []
        
        let messages = (timetableEntry["messages"] as? [[String: Any]] ?? []).compactMap { message -> Information in
            Information(
                type: .MESSAGES,
                key: "nasty ahhh",
                text: message["text"] as! String,
                textShort: message["textShort"] as? String
            )
        }
        
        let disruptions = (timetableEntry["disruptions"] as? [[String: Any]] ?? []).compactMap { disruption -> Information? in
            guard
                let descriptions = disruption["descriptions"] as? [String: [String: Any]],
                let firstObject = descriptions.first?.value,
                let text = firstObject["text"] as? String,
                let textShort = firstObject["textShort"] as? String
            else {
                return nil
            }
            
            return Information(
                type: .DISRUPTION,
                key: "general-warning",
                text: text,
                textShort: textShort
            )
        }
        
        let attributes = (timetableEntry["attributes"] as? [[String: Any]] ?? []).compactMap { attribute -> Information? in
            Information(
                type: .JOURNEY_ATTRIBUTE,
                key: attribute["code"] as! String,
                text: attribute["text"] as! String,
                textShort: attribute["textShort"] as? String
            )
        }
        
        informations.append(contentsOf: messages)
        informations.append(contentsOf: disruptions)
        informations.append(contentsOf: attributes)
        return informations
    }
    
    private func simplifyDescription(for journeyDescription: String) -> String {
        return journeyDescription.replacingOccurrences(of: journeyPattern, with: "", options: .regularExpression)
    }
}
