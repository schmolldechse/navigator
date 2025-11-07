//
//  Application+WorkerService.swift
//  backend
//
//  Created by Christian Knapp on 07.11.25.
//

import Vapor

fileprivate struct WorkerserviceKey: StorageKey {
    typealias Value = WorkerService
}

struct WorkerServiceLifecycleHandler: LifecycleHandler {
    func shutdown(_ application: Application) {
        application.workerService.cancelAll()
    }
}

extension Application {
    var workerService: WorkerService {
        get {
            if let existing = self.storage[WorkerserviceKey.self] {
                return existing
            } else {
                let workerService = WorkerService(application: self)
                self.storage[WorkerserviceKey.self] = workerService
                return workerService
            }
        }
        set {
            self.storage[WorkerserviceKey.self] = newValue
        }
    }
    
    func configureWorkerService() {
        let service = WorkerService(application: self)
        self.workerService = service
        
        self.workerService.start(DatabaseSizeWorker(application: self))
        
        self.lifecycle.use(WorkerServiceLifecycleHandler())
    }
}
