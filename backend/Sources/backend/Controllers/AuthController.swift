//
//  AuthController.swift
//  backend
//
//  Created by Christian Knapp on 28.10.25.
//

import Vapor

final class AuthController: RouteCollection, @unchecked Sendable {
    
    func boot(routes: any RoutesBuilder) throws {
        let auth = routes.grouped("auth")
            .groupedOpenAPI(tags: ["Authorization"])
            .excludeFromOpenAPI()
        
        auth.get("login", ":provider", use: handleLoginRedirect)
        auth.get("callback", ":provider", use: handleCallback)
        auth.get("logout", use: handleLogout)
        
        let userProtected = auth.grouped([User.sessionAuthenticator(.psql), User.guardMiddleware()])
        userProtected.get("me", use: getMe)
    }
    
    @Sendable
    func handleLoginRedirect(req: Request) async throws -> Response {
        let providerName = try req.parameters.require("provider")
        guard let provider = req.application.authRepositoryFactory.getProvider(named: providerName) else {
            req.logger.error("Unknown authentication provider requested: \(providerName)")
            throw Abort(.notFound, reason: "Unknown authentication provider: \(providerName)")
        }
        
        let redirectURL = try await provider.initiateAuth(req: req)
        return req.redirect(to: redirectURL)
    }
    
    @Sendable
    func handleCallback(req: Request) async throws -> Response {
        let providerName = try req.parameters.require("provider")
        guard let provider = req.application.authRepositoryFactory.getProvider(named: providerName) else {
            req.logger.error("Unknown authentication provider requested: \(providerName)")
            throw Abort(.notFound, reason: "Unknown authentication provider: \(providerName)")
        }
        
        let user = try await provider.handleCallback(req: req)
        req.auth.login(user)
        req.session.authenticate(user)
        return req.redirect(to: "http://localhost:5173")
    }
    
    func handleLogout(req: Request) async throws -> Response {
        req.auth.logout(User.self)
        req.session.destroy()
        return req.redirect(to: "http://localhost:5173")
    }
    
    func getMe(req: Request) async throws -> UserDTO {
        let user = try req.auth.require(User.self)
        return user.toDTO()
    }
}
