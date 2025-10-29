//
//  Migrations.swift
//  backend
//
//  Created by Christian Knapp on 05.11.25.
//

import Fluent
import Vapor

struct Migrations {
    static let all: [any Migration] = [
        SessionRecord.migration,
        StationMigration.init(),
        JourneyMigration.init(),
        IdentifiedRISIDMigration.init(),
        AuthMigration.init(),
        DatabaseSizeMigration.init()
    ]
    
    static func registerAll(on app: Application) {
        all.forEach { migration in
            app.migrations.add(migration)
        }
    }
}
