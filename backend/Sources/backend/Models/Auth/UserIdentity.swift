//
//  UserIdentity.swift
//  backend
//
//  Created by Christian Knapp on 28.10.25.
//

import Fluent
import Foundation

final class UserIdentity: Model, @unchecked Sendable {
    static let schema: String = "user_identities"
    static let space: String? = "auth"
    
    @ID(key: .id)
    var id: UUID?
    
    @Field(key: "provider")
    var provider: String
    
    @Field(key: "provider_id")
    var providerID: String
    
    @Parent(key: "user_id")
    var user: User
    
    @Timestamp(key: "created_at", on: .create)
    var createdAt: Date?
    
    init() { }
    
    init(id: UUID? = nil, provider: String, providerID: String, userID: UUID) {
        self.id = id
        self.provider = provider
        self.providerID = providerID
        self.$user.id = userID
    }
}
