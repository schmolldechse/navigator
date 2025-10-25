//  File.swift
//  backend
//
//  Created by Christian Knapp on 11.09.25.
//

import SwiftOpenAPI
import Vapor
import VaporToOpenAPI

internal struct DepartureEntry: Content {
    var ris_journeyId: String?
    var hafas_journeyId: String?

    var administration: AdministrationDTO?
    var transport: TransportDTO

    var destination: StopAtStopPlace
    var differingDestination: StopAtStopPlace?
    var direction: [StopPlace]
    var viaStops: [StopAtStopPlace]

    var departure: ScheduleAtStopPlaceDTO
    
    var informations: [InformationDTO]

    var cancelled: Bool
    var additional: Bool?
    var demand: Bool?
    
    var travelsWith: [CoupledTransport]?
}

internal struct ArrivalEntry: Content {
    var ris_journeyId: String?
    var hafas_journeyId: String?

    var administration: AdministrationDTO?
    var transport: TransportDTO

    var origin: StopAtStopPlace
    var differingOrigin: StopAtStopPlace?
    var direction: [StopPlace]
    var viaStops: [StopAtStopPlace]

    var arrival: ScheduleAtStopPlaceDTO
    
    var informations: [InformationDTO]

    var cancelled: Bool
    var additional: Bool?
    var demand: Bool?
    
    var travelsWith: [String]?
}

internal struct CoupledTransport: Content {
    var journeyId: String
    var separationAt: StopPlace
}

internal struct StopPlace: Content {
    var name: String
    var evaNumber: Int
}

internal struct StopAtStopPlace: Content {
    var name: String
    var evaNumber: Int
    var cancelled: Bool
    var additional: Bool?
}

internal struct ScheduleAtStopPlaceDTO: Content {
    var plannedTime: Date
    var actualTime: Date
    var delay: Int

    var plannedPlatform: String?
    var actualPlatform: String?
}

internal struct AdministrationDTO: Content {
    var administrationId: String
    var operatorCode: String
    var operatorName: String
}

internal enum InformationKeyDTO: String, CaseIterable, Codable {
    case UNPLANNED_INFO = "UNPLANNED_INFO"
    case GENERAL_WARNING = "GENERAL_WARNING"
    case ADDITIONAL_COACHES = "ADDITIONAL_COACHES"
    case MISSING_COACHES = "MISSING_COACHES"
    case REPLACEMENT_SERVICE = "REPLACEMENT_SERVICE"
    case CANCELLED_TRIP = "CANCELLED_TRIP"
    case ADDITIONAL_STOPS = "ADDITIONAL_STOPS"
    case NO_WI_FI = "NO_WI_FI"
    case CHANGED_SEQUENCE = "CHANGED_SEQUENCE"
    case NO_FIRST_CLASS = "NO_FIRST_CLASS"
    case ACCESSIBILITY_WARNING = "ACCESSIBILITY_WARNING"
    case RESERVATIONS_MISSING = "RESERVATIONS_MISSING"
    case RESERVATIONS_REQUIRED = "RESERVATIONS_REQUIRED"
    case NO_FOOD = "NO_FOOD"
    case NO_BICYCLE_TRANSPORT = "NO_BICYCLE_TRANSPORT"
    case BICYCLE_WARNING = "BICYCLE_WARNING"
    case BICYCLE_TRANSPORT = "BICYCLE_TRANSPORT"
    case BICYCLE_RESERVATION_REQUIRED = "BICYCLE_RESERVATION_REQUIRED"
    case TICKET_INFORMATION = "TICKET_INFORMATION"
}

internal struct InformationDTO: Content {
    var type: InformationType
    var key: InformationKeyDTO
    var text: String
    var textShort: String?
}

internal enum InformationType: String, CaseIterable, Codable {
    case JOURNEY_ATTRIBUTE = "JOURNEY_ATTRIBUTE"
    case DISRUPTION = "DISRUPTION"
    case MESSAGE = "MESSAGE"
    case RIS_QUALITY_DEVIATION = "RIS_QUALITY_DEVIATION"
    case RIS_CAUSE_REASON = "RIS_CAUSE_REASON"
}

internal struct TransportDTO: Content {
    var type: TransportType
    var replacementType: TransportType?
    var category: String

    var journeyType: JourneyType?
    var journeyDescription: String

    var number: Int
    var line: String?
}

internal enum TransportType: String, CaseIterable, Codable, OpenAPIDescriptable {
    case HIGH_SPEED_TRAIN = "HIGH_SPEED_TRAIN"
    case INTERCITY_TRAIN = "INTERCITY_TRAIN"
    case INTER_REGIONAL_TRAIN = "INTER_REGIONAL_TRAIN"
    case REGIONAL_TRAIN = "REGIONAL_TRAIN"
    case CITY_TRAIN = "CITY_TRAIN"
    case SUBWAY = "SUBWAY"
    case TRAM = "TRAM"
    case BUS = "BUS"
    case FERRY = "FERRY"
    case FLIGHT = "FLIGHT"
    case CAR = "CAR"
    case TAXI = "TAXI"
    case SHUTTLE = "SHUTTLE"
    case BIKE = "BIKE"
    case SCOOTER = "SCOOTER"
    case WALK = "WALK"
    case UNKNOWN = "UNKNOWN"
    
    static var openAPIDescription: (any OpenAPIDescriptionType)? {
        OpenAPIDescription<String>(
            """
            Type of transport.
            
            - HIGH_SPEED_TRAIN — High speed train (Hochgeschwindigkeitszug) like ICE or TGV, etc.
            - INTERCITY_TRAIN — Intercity train (Intercityzug)
            - INTER_REGIONAL_TRAIN — Interregional train (Interregiozug)
            - REGIONAL_TRAIN — Regional train (Regionalzug)
            - CITY_TRAIN — City train (S-Bahn)
            - SUBWAY — Subway (U-Bahn)
            - TRAM — Tram (Straßenbahn)
            - BUS — Bus
            - FERRY — Ferry (Fähre)
            - FLIGHT — Flight (Flugzeug)
            - CAR — Car (Auto)
            - TAXI — Taxi
            - SHUTTLE — Shuttle (Ruftaxi)
            - BIKE — (E-)Bike (Fahrrad)
            - SCOOTER — (E-)Scooter (Roller)
            - WALK — Walk (Laufen)
            - UNKNOWN — Unknown
            """
        )
    }
}

internal enum JourneyType: String, CaseIterable, Codable, OpenAPIDescriptable {
    case REGULAR = "REGULAR"
    case REPLACEMENT = "REPLACEMENT"
    case RELIEF = "RELIEF"
    case EXTRA = "EXTRA"
    
    static var openAPIDescription: (any OpenAPIDescriptionType)? {
        OpenAPIDescription<String>(
            """
            Defines whether journey [Fahrt] is regular or some kind of special.
            
            - REGULAR (Regular scheduled journey)
            - REPLACEMENT (Journey that replaces another journey)
            - RELIEF (Journey that reliefs another journey)
            - EXTRA (Journey that is somehow extra)
            """
        )
    }
}
