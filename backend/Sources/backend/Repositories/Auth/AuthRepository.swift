//
//  AuthRepository.swift
//  backend
//
//  Created by Christian Knapp on 28.10.25.
//

import Vapor

protocol AuthRepository {
    var providerName: String { get }
    
    func initiateAuth(req: Request) async throws -> String
    
    func handleCallback(req: Request) async throws -> User
}
