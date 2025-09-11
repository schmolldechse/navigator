//  File.swift
//  backend
//
//  Created by Christian Knapp on 11.09.25.
//

import Vapor
import VaporToOpenAPI
import SwiftOpenAPI

/// A timetable entry describing a single journey between two stops, including timing, journey information, intermediate stops and messages.
@OpenAPIDescriptable
struct TimetableEntryDTO: Content {
    /// The RIS (ReisendenInformationsSystem) identifier for the journey, if available. The first 8 characters (in 'yyyyMMdd' format) represents the date when the journey takes place.
    var ris_journeyId: String?
    /// The HAFAS identifier for the journey, if available.
    var hafas_journeyId: String?
    
    /// The origin stop of the journey.
    var origin: TimetableStopDTO
    /// The destination stop of the journey.
    var destination: TimetableStopDTO
    /// Intermediate stops (if any) between origin and destination in travel order.
    var viaStops: [TimetableStopDTO]
    
    /// Timing-related information (planned/actual times, delays, platforms).
    var timeInformation: TimeDTO
    /// Product and operator information for the journey.
    var journeyInformation: TimetableJourneyDTO
    
    /// Operational or passenger-facing messages related to this journey.
    var messages: [TimetableMessageDTO]
    /// Marks if the journey has been cancelled.
    var cancelled: Bool
}

/// A stop in the timetable, identified by name and EVA number.
@OpenAPIDescriptable
struct TimetableStopDTO: Content {
    /// The human-readable stop name.
    var name: String
    /// The number identifying the stop.
    var evaNumber: Int
}

/// Information about the product operating the journey.
@OpenAPIDescriptable
struct TimetableJourneyDTO: Content {
    /// The product type (e.g., NahverkehrsonstigeZuege, Sbahnen, ...).
    var productType: String
    /// The marketed product name (e.g., "ICE").
    var productName: String
    
    /// The unique journey number.
    var journeyNumber: Int
    /// A human-readable journey name or line designation.
    var journeyName: String
    
    /// The operating company’s name.
    var operatorName: String
}

/// A message related to the journey (disruption, info, hint).
@OpenAPIDescriptable
struct TimetableMessageDTO: Content {
    /// The message type or category.
    var type: String
    /// The message text presented to users.
    var text: String
}
