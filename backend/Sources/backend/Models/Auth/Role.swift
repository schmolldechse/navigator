//
//  Role.swift
//  backend
//
//  Created by Christian Knapp on 29.10.25.
//

import Fluent
import Foundation

final class Role: Model, @unchecked Sendable {
    static let schema: String = "roles"
    static let space: String? = "auth"
    
    @ID(key: .id)
    var id: UUID?
    
    @Field(key: "name")
    var name: String
    
    @Field(key: "weight")
    var weight: Int
    
    @Timestamp(key: "created_at", on: .create)
    var createdAt: Date?
    
    @Timestamp(key: "updated_at", on: .update)
    var updatedAt: Date?
    
    @Children(for: \.$role)
    var users: [User]
    
    init() { }
    
    init(id: UUID? = nil, name: String, weight: Int) {
        self.id = id
        self.name = name
        self.weight = weight
    }
}
