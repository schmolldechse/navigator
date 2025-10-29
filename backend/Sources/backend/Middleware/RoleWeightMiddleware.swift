//
//  RoleWeightMiddleware.swift
//  backend
//
//  Created by Christian Knapp on 29.10.25.
//

import Vapor

final class RoleWeightMiddleware: AsyncMiddleware {
    let minimumWeight: Int
    
    init(minimumWeight: Int) {
        self.minimumWeight = minimumWeight
    }
    
    func respond(to request: Request, chainingTo next: any AsyncResponder) async throws -> Response {
        guard let user = try? request.auth.require(User.self), let role = user.role else {
            throw Abort(.unauthorized, reason: "User not authenticated")
        }
        
        guard role.weight >= self.minimumWeight else {
            throw Abort(.forbidden, reason: "Insufficient permissions")
        }
        
        return try await next.respond(to: request)
    }
}
