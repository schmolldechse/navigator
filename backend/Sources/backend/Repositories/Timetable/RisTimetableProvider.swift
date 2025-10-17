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
    
    private func requestTimetable(for timetableRequest: TimetableRequest, isDeparture: Bool, req: Request) async throws -> [String: Any] {
        guard let clientId = Environment.get("BOARDS_CLIENT_ID"), let apiKey = Environment.get("BOARDS_API_KEY") else {
            throw Abort(.internalServerError, reason: "Missing Deutsche Bahn API credentials.")
        }
                
        let endTime: Date = timetableRequest.when.addingTimeInterval(Double(timetableRequest.duration) * 60.0)
        let response = try await req.client.get(URI(string: "\(apiUrl)/\(isDeparture ? "departures" : "arrivals")/\(timetableRequest.evaNumber)?timeEnd=\(endTime.ISO8601Format())&timeStart=\(timetableRequest.when.ISO8601Format())")) { clientReq in
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
    
    func retrieveDepartures(for timetableRequest: TimetableRequest, req: Request) async throws -> [DepartureEntry] {
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
    
    func retrieveArrivals(for timetableRequest: TimetableRequest, req: Request) async throws -> [ArrivalEntry] {
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
    
    private func buildSchedule(for timetableEntry: [String: Any]) -> ScheduleAtStopPlaceDTO {
        let plannedTime: Date = ((timetableEntry["timeSchedule"] as? String)?.toDate())!
        let actualTime: Date = ((timetableEntry["time"] as? String)?.toDate())!
        let delay: Int = Int(actualTime.timeIntervalSince(plannedTime))
        
        return ScheduleAtStopPlaceDTO(
            plannedTime: plannedTime,
            actualTime: actualTime,
            delay: delay,
            plannedPlatform: timetableEntry["platformSchedule"] as? String,
            actualPlatform: timetableEntry["platform"] as? String
        )
    }
    
    private func buildTransport(for transportEntry: [String: Any], type: String) -> TransportDTO {
        let replacementType: TransportType? = {
            guard let replacementTypeObj = transportEntry["replacementTransport"] as? [String: Any] else { return nil }
            return TransportType(rawValue: (replacementTypeObj["realType"] as! String).uppercased()) ?? .UNKNOWN
        }()
                
        return TransportDTO(
            type: TransportType(rawValue: (transportEntry["type"] as! String).uppercased()) ?? .UNKNOWN,
            replacementType: replacementType,
            category: transportEntry["category"] as! String,
            journeyType: JourneyType(rawValue: type.uppercased())!,
            journeyDescription: transportEntry["journeyDescription"] as! String,
            number: transportEntry["number"] as! Int,
            line: transportEntry["line"] as? String
        )
    }
    
    private func buildAdministration(for timetableEntry: [String: Any]) -> AdministrationDTO? {
        guard let administration = timetableEntry["administration"] as? [String: Any] else {
            return nil
        }
        
        return AdministrationDTO(
            administrationId: administration["administrationID"] as! String,
            operatorCode: administration["operatorCode"] as! String,
            operatorName: administration["operatorName"] as! String
        )
    }
    
    private func makeMessageKey(for originalMessageKey: String) -> InformationKeyDTO {
        switch originalMessageKey {
        case "1", "13", "23", "24", "27", "32", "42", "43", "44", "45", "47", "48", "51", "62", "63", "68", "69", "94", "CK", "EF", "EH", "FT", "HS", "OA", "OC", "RG", "RO", "SI", "SM":
            return InformationKeyDTO.UNPLANNED_INFO
        case "2", "3", "5", "6", "7", "8", "9", "10", "11", "12", "14", "15", "16", "17", "18", "19", "21", "22", "28", "31", "33", "34", "35", "36", "38", "39", "40", "41", "49", "50", "52", "53", "54", "55", "58", "59", "60", "61", "64", "65", "66", "67", "72", "96", "97", "98", "99", "1000":
            return InformationKeyDTO.GENERAL_WARNING
        case "25":
            return InformationKeyDTO.ADDITIONAL_COACHES
        case "26", "79", "82", "85":
            return InformationKeyDTO.MISSING_COACHES
        case "37":
            return InformationKeyDTO.CANCELLED_TRIP
        case "57":
            return InformationKeyDTO.ADDITIONAL_STOPS
        case "70", "71":
            return InformationKeyDTO.NO_WI_FI
        case "73", "74", "75", "76", "80", "81":
            return InformationKeyDTO.CHANGED_SEQUENCE
        case "77":
            return InformationKeyDTO.NO_FIRST_CLASS
        case "29", "78":
            return InformationKeyDTO.REPLACEMENT_SERVICE
        case "83", "93", "95", "DC", "OG":
            return InformationKeyDTO.ACCESSIBILITY_WARNING
        case "86", "87":
            return InformationKeyDTO.RESERVATIONS_MISSING
        case "RP":
            return InformationKeyDTO.RESERVATIONS_REQUIRED
        case "90":
            return InformationKeyDTO.NO_FOOD
        case "91", "NF":
            return InformationKeyDTO.NO_BICYCLE_TRANSPORT
        case "92", "FB", "FK", "FS", "G":
            return InformationKeyDTO.BICYCLE_WARNING
        case "AB", "KF", "RF", "TF":
            return InformationKeyDTO.BICYCLE_TRANSPORT
        case "FF", "FO", "FR":
            return InformationKeyDTO.BICYCLE_RESERVATION_REQUIRED
        case "N+", "NG", "NJ":
            return InformationKeyDTO.TICKET_INFORMATION
        default:
            return InformationKeyDTO.UNPLANNED_INFO
        }
    }
    
    private func buildInformations(for timetableEntry: [String: Any]) -> [InformationDTO] {
        var informations: [InformationDTO] = []
        
        let messages = (timetableEntry["messages"] as? [[String: Any]] ?? []).compactMap { message -> InformationDTO? in
            guard let keyStr = message["key"] as? String else { return nil }
            return InformationDTO(
                type: .MESSAGE,
                key: self.makeMessageKey(for: keyStr),
                text: message["text"] as! String,
                textShort: message["textShort"] as? String
            )
        }
        
        let disruptions = (timetableEntry["disruptions"] as? [[String: Any]] ?? []).compactMap { disruption -> InformationDTO? in
            guard
                let descriptions = disruption["descriptions"] as? [String: [String: Any]],
                let firstObject = descriptions.first?.value,
                let text = firstObject["text"] as? String,
                let textShort = firstObject["textShort"] as? String
            else {
                return nil
            }
            
            return InformationDTO(
                type: .DISRUPTION,
                key: .GENERAL_WARNING,
                text: text,
                textShort: textShort
            )
        }
        
        let attributes = (timetableEntry["attributes"] as? [[String: Any]] ?? []).compactMap { attribute -> InformationDTO? in
            guard let code = attribute["code"] as? String else { return nil }
            return InformationDTO(
                type: .JOURNEY_ATTRIBUTE,
                key: self.makeMessageKey(for: code),
                text: attribute["text"] as! String,
                textShort: attribute["textShort"] as? String
            )
        }
        
        informations.append(contentsOf: messages)
        informations.append(contentsOf: disruptions)
        informations.append(contentsOf: attributes)
        return informations
    }
}

