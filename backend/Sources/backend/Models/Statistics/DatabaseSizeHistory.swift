//
//  DatabaseSizeHistory.swift
//  backend
//
//  Created by Christian Knapp on 05.11.25.
//

import Fluent
import Foundation

final class DatabaseSizeHistory: Model, @unchecked Sendable {
    static let schema: String = "size_history"
    static let space: String? = "statistics"
    
    @ID(custom: "id", generatedBy: .database)
    var id: Int?
    
    @Field(key: "recorded_at")
    var recordedAt: Date
    
    @Field(key: "size_bytes")
    var sizeBytes: Int64
    
    init() { }
    
    init(id: Int? = nil, recordedAt: Date, sizeBytes: Int64) {
        self.id = id
        self.recordedAt = recordedAt
        self.sizeBytes = sizeBytes
    }
}
