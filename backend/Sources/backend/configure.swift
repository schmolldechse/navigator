import NIOSSL
import Fluent
import FluentPostgresDriver
import Leaf
import Vapor

// configures your application
public func configure(_ app: Application) async throws {
    // uncomment to serve files from /Public folder
    // app.middleware.use(FileMiddleware(publicDirectory: app.directory.publicDirectory))
    
    // MARK: - JSON Date Formatting Configuration
    let jsonEncoder = JSONEncoder()
    let jsonDecoder = JSONDecoder()
    
    let dateFormatter = DateFormatter()
    dateFormatter.locale = Locale(identifier: "de_DE")
    dateFormatter.timeZone = TimeZone(identifier: "Europe/Berlin")!
    dateFormatter.dateFormat = "yyyy-MM-dd'T'HH:mm:ssXXX"

    jsonEncoder.dateEncodingStrategy = .formatted(dateFormatter)
    jsonDecoder.dateDecodingStrategy = .formatted(dateFormatter)

    ContentConfiguration.global.use(encoder: jsonEncoder, for: .json)
    ContentConfiguration.global.use(decoder: jsonDecoder, for: .json)

    app.databases.use(DatabaseConfigurationFactory.postgres(configuration: .init(
        hostname: Environment.get("DATABASE_HOST") ?? "localhost",
        port: Environment.get("DATABASE_PORT").flatMap(Int.init(_:)) ?? SQLPostgresConfiguration.ianaPortNumber,
        username: Environment.get("DATABASE_USERNAME") ?? "vapor_username",
        password: Environment.get("DATABASE_PASSWORD") ?? "vapor_password",
        database: Environment.get("DATABASE_NAME") ?? "vapor_database",
        tls: .prefer(try .init(configuration: .clientDefault)))
    ), as: .psql)
    
    app.views.use(.leaf)
    
    // MARK: - Workers
    app.configureWorkerService()
        
    // MARK: - Sessions
    app.sessions.use(.fluent(.psql))
    app.sessions.configuration.cookieFactory = { sessionID in
            .init(string: sessionID.string, isSecure: app.environment.isRelease, sameSite: .lax)
    }
    
    // MARK: - Migrations
    Migrations.registerAll(on: app)
    if !app.environment.isRelease {
        app.logger.info("Running migrations...")
        try await app.autoMigrate()
        app.logger.info("Migrations complete.")
    }
    
    // MARK: - Middleware
    app.middleware.use(app.sessions.middleware)
    app.middleware.use(RequestIdentificationErrorMiddleware())
    
    // register routes
    try routes(app)
}
