//
//  UserDTO.swift
//  backend
//
//  Created by Christian Knapp on 28.10.25.
//

import Vapor
import VaporToOpenAPI

@OpenAPIDescriptable
struct UserDTO: Content {
    /// The unique identifier of the user.
    let id: UUID
    /// The username of the user.
    let username: String
    /// The email address of the user.
    let email: String?
    /// The date when the user was created.
    let createdAt: Date
    /// The date when the user was last updated.
    let updatedAt: Date
    /// The users profile picture.
    let profilePicture: String?
}
