/**
 * Type of transport.
 *
 * - HIGH_SPEED_TRAIN — High speed train (Hochgeschwindigkeitszug) like ICE or TGV, etc.
 * - INTERCITY_TRAIN — Intercity train (Intercityzug)
 * - INTER_REGIONAL_TRAIN — Interregional train (Interregiozug)
 * - REGIONAL_TRAIN — Regional train (Regionalzug)
 * - CITY_TRAIN — City train (S-Bahn)
 * - SUBWAY — Subway (U-Bahn)
 * - TRAM — Tram (Straßenbahn)
 * - BUS — Bus
 * - FERRY — Ferry (Fähre)
 * - FLIGHT — Flight (Flugzeug)
 * - CAR — Car (Auto)
 * - TAXI — Taxi
 * - SHUTTLE — Shuttle (Ruftaxi)
 * - BIKE — (E-)Bike (Fahrrad)
 * - SCOOTER — (E-)Scooter (Roller)
 * - WALK — Walk (Laufen)
 * - UNKNOWN — Unknown
 * @export
 */
export const TransportType = {
	HighSpeedTrain: "HIGH_SPEED_TRAIN",
	IntercityTrain: "INTERCITY_TRAIN",
	InterRegionalTrain: "INTER_REGIONAL_TRAIN",
	RegionalTrain: "REGIONAL_TRAIN",
	CityTrain: "CITY_TRAIN",
	Subway: "SUBWAY",
	Tram: "TRAM",
	Bus: "BUS",
	Ferry: "FERRY",
	Flight: "FLIGHT",
	Car: "CAR",
	Taxi: "TAXI",
	Shuttle: "SHUTTLE",
	Bike: "BIKE",
	Scooter: "SCOOTER",
	Walk: "WALK",
	Unknown: "UNKNOWN"
} as const;
export type TransportType = (typeof TransportType)[keyof typeof TransportType];
