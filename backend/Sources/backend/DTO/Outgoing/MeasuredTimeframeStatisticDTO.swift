//
//  MeasuredTimeframeStatisticDTO.swift
//  backend
//
//  Created by Christian Knapp on 08.11.25.
//

import Vapor

struct MeasuredTimeframeStatisticDTO: Content {
    let timeframe: DateRangeDTO
    let total: Int64
    let change: Int64
    let unit: MeasurementUnit
    let values: [MeasuredValueDTO]
}

struct DateRangeDTO: Content {
    let start: Date
    let end: Date
}

struct MeasuredValueDTO: Content {
    let date: Date
    let value: Int64
}

enum MeasurementUnit: String, Content {
    case bytes
    case count
    case percentage
}
