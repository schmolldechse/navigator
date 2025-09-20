//
//  RequestIdMiddleware.swift
//  backend
//
//  Created by Christian Knapp on 20.09.25.
//

import Foundation
import Vapor

struct RequestIdentificationError: Codable, Content {
    var reason: String
    var requestId: String
}

final class RequestIdentificationErrorMiddleware: Middleware {
    func respond(to request: Request, chainingTo next: any Responder) -> EventLoopFuture<Response> {
        return next.respond(to: request).flatMapErrorThrowing { error in
            let status: HTTPStatus
            let reason: String
            
            if let abort = error as? (any AbortError) {
                status = abort.status
                reason = abort.reason
            } else {
                status = .internalServerError
                reason = "An unexpected error occurred. Please try again later."
            }
            
            let errorPayload = RequestIdentificationError(reason: reason, requestId: request.id)
            let response = Response(status: status)
            try response.content.encode(errorPayload, as: .json)
            return response
        }
    }
}
