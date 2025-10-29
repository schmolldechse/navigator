import Fluent
import Vapor
import VaporToOpenAPI

func routes(_ app: Application) throws {
    app.get("") { req async in
        req.redirect(to: "swagger", redirectType: .permanent)
    }
    .excludeFromOpenAPI()
    
    app.get("swagger") { req async throws in
        try await req.view.render("index", ["title": "Navigator Backend"])
    }
    .excludeFromOpenAPI()

    app.routes.get("swagger.json") { req in
        req.application.routes.openAPI(
            info: InfoObject(
                title: "Navigator Backend",
                description: "API for Navigator Backend", 
                version: "1.0"
            )
        )
    }
    .excludeFromOpenAPI()

    let api = app.grouped("api", "v1")
    try api.register(collection: StationController())
    try api.register(collection: TimetableController())
    try api.register(collection: AuthController())
}
