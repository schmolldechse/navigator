//
//  File.swift
//  backend
//
//  Created by Christian Knapp on 05.09.25.
//

import Vapor
import Foundation

protocol TimetableProvider {
    func retrieveDepartures(for timetableRequest: TimetableRequestDTO, req: Request) async throws -> [DepartureEntry]
    
    func retrieveArrivals(for timetableRequest: TimetableRequestDTO, req: Request) async throws -> [ArrivalEntry]
}
