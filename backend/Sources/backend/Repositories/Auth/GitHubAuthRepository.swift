//
//  GitHubAuthRepository.swift
//  backend
//
//  Created by Christian Knapp on 28.10.25.
//

import Vapor

final class GitHubAuthRepository: AuthRepository {
    let providerName: String = "github"
    
    private let clientId: String
    private let clientSecret: String
    private let redirectURI: String
    
    private let githubAuthorizeURL = "https://github.com/login/oauth/authorize"
    private let githubTokenURL = "https://github.com/login/oauth/access_token"
    private let githubUserURL = "https://api.github.com/user"
    
    init() {
        guard let clientId = Environment.get("GITHUB_CLIENT_ID"), let clientSecret = Environment.get("GITHUB_CLIENT_SECRET") else {
            fatalError("GitHub OAuth credentials are not set in environment variables.")
        }
        
        self.clientId = clientId
        self.clientSecret = clientSecret
        
        self.redirectURI = "http://localhost:8080/api/v1/auth/callback/github"
    }
    
    func handleCallback(req: Request) async throws -> User {        
        guard let code: String = req.query["code"] else {
            req.logger.error("Missing authorization code in GitHub callback.")
            throw Abort(.badRequest, reason: "Missing authorization code in callback.")
        }
        
        let accessToken = try await getAccessToken(req: req, code: code)
        let githubUser = try await getGitHubUser(req: req, accessToken: accessToken)
        
        return try await findOrCreateUser(req: req, githubUser: githubUser)
    }
    
    func initiateAuth(req: Request) async throws -> String {
        var components = URLComponents(string: githubAuthorizeURL)!
        components.queryItems = [
            URLQueryItem(name: "client_id", value: clientId),
            URLQueryItem(name: "redirect_uri", value: redirectURI),
            URLQueryItem(name: "scope", value: "read:user user:email"),
            URLQueryItem(name: "state", value: [UInt].random(count: 16).map { String(format: "%02x", $0) }.joined())
        ]
        
        guard let url = components.string else {
            req.logger.error("Failed to construct GitHub authorization URL.")
            throw Abort(.internalServerError, reason: "Failed to construct GitHub authorization URL.")
        }
        
        return url
    }
    
    private func getAccessToken(req: Request, code: String) async throws -> String {
        let response = try await req.client.post(URI(string: self.githubTokenURL)) { clientReq in
            try clientReq.content.encode([
                "client_id": self.clientId,
                "client_secret": self.clientSecret,
                "code": code,
            ], as: .urlEncodedForm)
            clientReq.headers.add(name: "Accept", value: "application/json")
        }
        
        struct GitHubTokenResponse: Content {
            let access_token: String
        }
        
        let tokenResponse = try response.content.decode(GitHubTokenResponse.self)
        return tokenResponse.access_token
    }
    
    private func getGitHubUser(req: Request, accessToken: String) async throws -> GitHubUserResponse {
        let response = try await req.client.get(URI(string: self.githubUserURL)) { clientReq in
            clientReq.headers.bearerAuthorization = BearerAuthorization(token: accessToken)
            clientReq.headers.add(name: .userAgent, value: "Navigator")
        }
        return try response.content.decode(GitHubUserResponse.self)
    }
    
    private func findOrCreateUser(req: Request, githubUser: GitHubUserResponse) async throws -> User {
        if let identity = try await UserIdentity.query(on: req.db)
            .filter(\UserIdentity.$provider, .equal, self.providerName)
            .filter(\UserIdentity.$providerID, .equal, String(githubUser.id))
            .with(\.$user)
            .first() {
            return identity.user
        }
        
        let user = User(
            username: githubUser.login,
            email: githubUser.email,
            profilePicture: githubUser.avatar_url
        )
        try await user.save(on: req.db)
        
        guard let userId = user.id else {
            req.logger.error("Failed to retrieve user ID after saving new user.")
            throw Abort(.internalServerError, reason: "Failed to create user.")
        }
        
        let identity = UserIdentity(
            provider: self.providerName,
            providerID: String(githubUser.id),
            userID: userId
        )
        try await identity.save(on: req.db)
        
        return user
    }
    
    private struct GitHubUserResponse: Content {
        let id: Int
        let login: String
        let avatar_url: String
        let email: String?
    }
}
