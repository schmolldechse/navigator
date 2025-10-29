//
//  User.swift
//  backend
//
//  Created by Christian Knapp on 28.10.25.
//

import Fluent
import Foundation

final class User: Model, @unchecked Sendable {
    static let schema: String = "users"
    static let space: String? = "auth"
    
    @ID(key: .id)
    var id: UUID?
    
    @Field(key: "username")
    var username: String
    
    @OptionalField(key: "email")
    var email: String?
    
    @OptionalField(key: "profile_picture")
    var profilePicture: String?
    
    @Timestamp(key: "created_at", on: .create)
    var createdAt: Date?
    
    @Timestamp(key: "updated_at", on: .update)
    var updatedAt: Date?
    
    @Children(for: \.$user)
    var identities: [UserIdentity]
    
    @OptionalParent(key: "role_id")
    var role: Role?
    
    init() { }
    
    init(id: UUID? = nil, username: String, email: String? = nil, profilePicture: String? = nil) {
        self.id = id
        self.username = username
        self.email = email
        self.profilePicture = profilePicture
    }
    
    func toDTO() -> UserDTO {
        UserDTO(
            id: self.id!,
            username: self.username,
            email: self.email,
            createdAt: self.createdAt!,
            updatedAt: self.updatedAt!,
            profilePicture: self.profilePicture
        )
    }
}

extension User: ModelSessionAuthenticatable {
    var sessionID: UUID {
        self.id!
    }
}
