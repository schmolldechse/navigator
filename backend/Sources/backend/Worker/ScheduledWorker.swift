//
//  ScheduledWorker.swift
//  backend
//
//  Created by Christian Knapp on 07.11.25.
//

import NIOCore
import Vapor

protocol ScheduledWorker: Sendable {
    var name: String { get }
    var interval: TimeAmount { get }
          
    init(application: Application)
    
    func execute() async throws
}
