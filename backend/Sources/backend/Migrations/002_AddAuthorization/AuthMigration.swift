//
//  AuthMigration.swift
//  backend
//
//  Created by Christian Knapp on 28.10.25.
//

import Fluent
import SQLKit

struct AuthMigration: AsyncMigration {
    func prepare(on database: any Database) async throws {
        guard let sql = database as? (any SQLDatabase) else { return }
        
        // schema
        try await sql.raw("""
            CREATE SCHEMA IF NOT EXISTS "auth"; 
        """).run()
        
        // roles
        try await sql.raw("""
            CREATE TABLE IF NOT EXISTS "\(unsafeRaw: Role.space!)"."\(unsafeRaw: Role.schema)" (
                "id" uuid PRIMARY KEY,
                "name" varchar(255) UNIQUE NOT NULL,
                "weight" integer NOT NULL,
                "created_at" timestamp NOT NULL DEFAULT now(),
                "updated_at" timestamp NOT NULL DEFAULT now()
            );
            """).run()
        
        // users
        try await sql.raw("""
            CREATE TABLE IF NOT EXISTS "\(unsafeRaw: User.space!)"."\(unsafeRaw: User.schema)" (
                "id" uuid PRIMARY KEY,
                "username" varchar(255) UNIQUE NOT NULL,
                "email" varchar(255) UNIQUE,
                "profile_picture" varchar(2048),
                "created_at" timestamp NOT NULL DEFAULT now(),
                "updated_at" timestamp NOT NULL DEFAULT now(),
                "role_id" uuid REFERENCES "\(unsafeRaw: Role.space!)"."\(unsafeRaw: Role.schema)" ("id") ON DELETE SET NULL
            );
            """).run()
        
        // user_identities
        try await sql.raw("""
            CREATE TABLE IF NOT EXISTS "\(unsafeRaw: UserIdentity.space!)"."\(unsafeRaw: UserIdentity.schema)" (
                "id" uuid PRIMARY KEY,
                "provider" varchar(255) NOT NULL,
                "provider_id" varchar(255) NOT NULL,
                "user_id" uuid REFERENCES "\(unsafeRaw: User.space!)"."\(unsafeRaw: User.schema)" ("id") ON DELETE CASCADE,
                "created_at" timestamp NOT NULL DEFAULT now(),
                CONSTRAINT "unique_provider_identity" UNIQUE ("provider", "provider_id")
            );
            """).run()
    }
    
    func revert(on database: any Database) async throws {
        guard let sql = database as? (any SQLDatabase) else { return }
        
        // user_identities
        try await sql.raw("""
            DROP TABLE IF EXISTS "\(unsafeRaw: UserIdentity.space!)"."\(unsafeRaw: UserIdentity.schema)";
            """).run()
        
        // users
        try await sql.raw("""
            DROP TABLE IF EXISTS "\(unsafeRaw: User.space!)"."\(unsafeRaw: User.schema)";
            """).run()
        
        // roles
        try await sql.raw("""
            DROP TABLE IF EXISTS "\(unsafeRaw: Role.space!)"."\(unsafeRaw: Role.schema)";
            """).run()
        
        // auth
        try await sql.raw("""
            DROP SCHEMA IF EXISTS "auth";
        """).run()
    }
}
