//
//  Administration.swift
//  backend
//
//  Created by Christian Knapp on 21.09.25.
//

import Fluent

final class Administration: Model, @unchecked Sendable {
    static let schema: String = "journey_administrations"
    static let space: String? = "core"
    
    @ID(custom: "id", generatedBy: .database)
    var id: Int?
    
    @Field(key: "administration_id")
    var administrationId: String
    
    @Field(key: "operator_code")
    var operatorCode: String
    
    @Field(key: "operator_name")
    var operatorName: String
    
    init() { }
    
    init(id: Int? = nil, administrationId: String, operatorCode: String, operatorName: String) {
        self.id = id
        self.administrationId = administrationId
        self.operatorCode = operatorCode
        self.operatorName = operatorName
    }
}
