import Fluent
import Vapor
import VaporToOpenAPI

func routes(_ app: Application) throws {
    app.get("") { req async in
        req.redirect(to: "swagger", redirectType: .permanent)
    }
    
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

    try app.register(collection: TodoController())
    try app.register(collection: TimetableController())
}
