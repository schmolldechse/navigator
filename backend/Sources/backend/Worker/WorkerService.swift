//
//  WorkerService.swift
//  backend
//
//  Created by Christian Knapp on 07.11.25.
//

import Foundation
import NIOConcurrencyHelpers
import NIOCore
import Vapor

final class WorkerService: Sendable {
    private let runningWorkers: NIOLockedValueBox<[UUID: RepeatedTask]>
    private let application: Application

    init(application: Application) {
        self.runningWorkers = .init([:])
        self.application = application
    }

    @discardableResult
    func start(_ worker: any ScheduledWorker) -> UUID {
        let id = UUID()
        let eventLoop = self.application.eventLoopGroup.any()

        let task = eventLoop.scheduleRepeatedTask(
            initialDelay: .zero,
            delay: worker.interval
        ) { [weak self] (task: RepeatedTask) in
            guard let self = self else {
                task.cancel()
                return
            }

            Task {
                try await worker.execute()
            }
        }

        self.runningWorkers.withLockedValue { $0[id] = task }
        return id
    }
    
    func cancel(id: UUID) {
        let task = self.runningWorkers.withLockedValue { $0.removeValue(forKey: id) }
        if let task = task {
            task.cancel()
        }
    }
    
    func cancelAll() {
        let tasks = self.runningWorkers.withLockedValue { tasks in
            let allTasks = Array(tasks.values)
            tasks.removeAll()
            return allTasks
        }
        
        tasks.forEach { $0.cancel() }
    }
    
    var logger: Logger {
        self.application.logger
    }
}
