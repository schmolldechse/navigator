//
//  AuthRepositoryFactory.swift
//  backend
//
//  Created by Christian Knapp on 28.10.25.
//

struct AuthRepositoryFactory: @unchecked Sendable {
    private let providers: [String: any AuthRepository]
    
    init() {
        self.providers = ["github": GitHubAuthRepository()]
    }
    
    func getProvider(named name: String) -> (any AuthRepository)? {
        return providers[name.lowercased()]
    }
}
