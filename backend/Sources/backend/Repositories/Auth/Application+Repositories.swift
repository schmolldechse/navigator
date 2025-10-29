//
//  Application+AuthRepositoryFactory.swift
//  backend
//
//  Created by Christian Knapp on 28.10.25.
//

import Vapor

struct AuthRepositoryFactoryKey: StorageKey {
    typealias Value = AuthRepositoryFactory
}

extension Application {
    var authRepositoryFactory: AuthRepositoryFactory {
        get {
            if let existing = self.storage[AuthRepositoryFactoryKey.self] {
                return existing
            } else {
                let factory = AuthRepositoryFactory()
                self.storage[AuthRepositoryFactoryKey.self] = factory
                return factory
            }
        }
        set {
            self.storage[AuthRepositoryFactoryKey.self] = newValue
        }
    }
    
    var stationRepository: StationRepository {
        StationRepository(client: self.client, logger: self.logger, database: self.db)
    }
}
