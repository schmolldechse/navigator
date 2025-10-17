//
//  File.swift
//  backend
//
//  Created by Christian Knapp on 05.09.25.
//

import Vapor
import Foundation

protocol TimetableProvider {
    func retrieveDepartures(for timetableRequest: TimetableRequest, req: Request) async throws -> [DepartureEntry]
    
    func retrieveArrivals(for timetableRequest: TimetableRequest, req: Request) async throws -> [ArrivalEntry]
}
