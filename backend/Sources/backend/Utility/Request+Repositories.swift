//
// Request+Repositories.swift
// backend
//
// Created by Christian Knapp on 17.10.25.
//

import Vapor

extension Request {
    var stationRepository: any StationRepository {
        VendoStationRepository(client: self.client, logger: self.logger, database: self.db)
    }
}
