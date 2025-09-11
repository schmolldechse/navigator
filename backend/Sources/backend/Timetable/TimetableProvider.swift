//
//  File.swift
//  backend
//
//  Created by Christian Knapp on 05.09.25.
//

import Vapor
import Foundation

protocol TimetableProvider {
    func retrieveTimetable(for timetableRequest: TimetableRequestDTO, req: Request) async throws -> [TimetableEntryDTO]
}

