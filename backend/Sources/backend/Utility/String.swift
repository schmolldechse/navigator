//
//  String.swift
//  backend
//
//  Created by Christian Knapp on 14.09.25.
//

import Foundation

extension String {
    func toDate() -> Date? {
        let formatter = DateFormatter()
        formatter.dateFormat = "yyyy-MM-dd'T'HH:mm:ssXXX"
        return formatter.date(from: self)
    }
}
