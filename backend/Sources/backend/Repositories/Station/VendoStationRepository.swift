//
// VendoStationRepository.swift
// backend
//
// Created by Christian Knapp on 17.10.25.
//

import Vapor
import Fluent

struct VendoStationRepository: StationRepository {
    let client: any Client
    let logger: Logger
    let database: any Database

    func query(searchTerm: String) async throws -> [StationDTO] {
        let response = try await client.post(URI(string: "https://app.vendo.noncd.db.de/mob/location/search")) { clientReq in
            try clientReq.content.encode(VendoStationSearchRequestDTO(searchTerm: searchTerm))
            
            clientReq.headers.add(name: "X-Correlation-ID", value: "\(UUID().uuidString)_\(UUID().uuidString)")
            clientReq.headers.add(name: "Accept", value: "application/x.db.vendo.mob.location.v3+json")
            clientReq.headers.replaceOrAdd(name: "Content-Type", value: "application/x.db.vendo.mob.location.v3+json")
        }
        
        guard response.status == .ok else {
            logger.error("Vendo station search failed with status: \(response.status.code) - \(response.status.description)")
            throw Abort(.badGateway, reason: "Failed to query stations from Vendo.")
        }

        guard let responseBody = response.body else {
            logger.error("Vendo station search response body is nil.")
            throw Abort(.internalServerError, reason: "Failed to parse the response body.")
        }
        
        guard let jsonObject = try? JSONSerialization.jsonObject(with: Data(buffer: responseBody)) as? [[String: Any]] else {
            throw Abort(.internalServerError, reason: "Failed to parse JSON response into a dictionary.")
        }
        
        var stations: [StationDTO] = []
        stations.reserveCapacity(jsonObject.count)
        
        for stationEntry in jsonObject {
            guard let evaString = stationEntry["evaNr"] as? String,
                  let evaNumber = Int(evaString),
                  let name = stationEntry["name"] as? String,
                  let coordinates = stationEntry["coordinates"] as? [String: Double],
                  let latitude = coordinates["latitude"],
                  let longitude = coordinates["longitude"],
                  let products = stationEntry["products"] as? [String]
            else {
                logger.warning("Skipping station entry due to missing or invalid data: \(stationEntry)")
                continue
            }
            
            let ril100 = try await Ril100.query(on: self.database)
                .filter(\.$station.$id == evaNumber)
                .all()
                .map { $0.ril100 }
            
            var transports: [TransportType] = try await TransportOccurence.query(on: self.database)
                .filter(\.$station.$id == evaNumber)
                .all()
                .map { $0.transport }
            transports += products.map { mapToRisTransport(for: $0) }
            transports = Array(Set(transports))
            
            let position: PositionDTO = PositionDTO(latitude: latitude, longitude: longitude)
            stations.append(StationDTO(
                evaNumber: evaNumber,
                name: name,
                position: position,
                ril100: ril100,
                transports: transports
            ))
        }
        
        return stations
    }
    
    func findByEvaNumber(evaNumber: Int, on req: Request) async throws -> StationDTO? {
        return nil
    }
    
    private func mapToRisTransport(for transportName: String) -> TransportType {
        switch transportName.uppercased() {
        case "HOCHGESCHWINDIGKEITSZUEGE":
            return .HIGH_SPEED_TRAIN
        case "INTERCITYUNDEUROCITYZUEGE":
            return .INTERCITY_TRAIN
        case "INTERREGIOUNDSCHNELLZUEGE":
            return .INTER_REGIONAL_TRAIN
        case "NAHVERKEHRSONSTIGEZUEGE":
            return .REGIONAL_TRAIN
        case "SBAHNEN":
            return .CITY_TRAIN
        case "BUSSE":
            return .BUS
        case "SCHIFFE":
            return .FERRY
        case "UBAHN":
            return .SUBWAY
        case "STRASSENBAHN":
            return .TRAM
        case "ANRUFPFLICHTIGEVERKEHRE":
            return .SHUTTLE
        default:
            return .UNKNOWN
        }
    }
}
