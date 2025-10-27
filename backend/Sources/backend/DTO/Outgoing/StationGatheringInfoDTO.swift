//
//  StationGatheringInfoDTO.swift
//  backend
//
//  Created by Christian Knapp on 27.10.25.
//

import Vapor
import VaporToOpenAPI

@OpenAPIDescriptable
struct StationGatheringInfoDTO: Content {
    /// Indicates if querying for this station is enabled.
    let queryingEnabled: Bool
    /// Timestamp of the last successful querying for this station; `nil` if never queried.
    let lastQueried: Date?
    /// Transport types enabled for RIS ID discovery; only journeys with these types are inserted as unique RIS IDs.
    let active: [TransportType]
    /// Transport types ignored during RIS ID discovery; journeys with these types are skipped and no RIS IDs are created.
    let inactive: [TransportType]
}
