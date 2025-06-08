enum Products {
	HOCHGESCHWINDIGKEITSZUEGE = "HOCHGESCHWINDIGKEITSZUEGE",
	INTERCITYUNDEUROCITYZUEGE = "INTERCITYUNDEUROCITYZUEGE",
	INTERREGIOUNDSCHNELLZUEGE = "INTERREGIOUNDSCHNELLZUEGE",
	NAHVERKEHRSONSTIGEZUEGE = "NAHVERKEHRSONSTIGEZUEGE",
	SBAHNEN = "SBAHNEN",
	BUSSE = "BUSSE",
	SCHIFFE = "SCHIFFE",
	UBAHN = "UBAHN",
	STRASSENBAHN = "STRASSENBAHN",
	ANRUFPFLICHTIGEVERKEHRE = "ANRUFPFLICHTIGEVERKEHRE",
	UNKNOWN = "UNKNOWN"
}

const mapToProduct = (input?: string): string => {
	if (!input) return Products.UNKNOWN;

	switch (input.toLowerCase()) {
		case "high_speed_train": // origin: HIGH_SPEED_TRAIN : bahnhof.de, regio-guide.de
		case "nationalexpress": // origin: nationalExpress :
		case "ice": // origin: ICE : bahn.de
			return Products.HOCHGESCHWINDIGKEITSZUEGE;
		case "intercity_train": // origin: INTERCITY_TRAIN : bahnhof.de, regio-guide.de
		case "national": // origin: national :
		case "ec_ic": // origin: EC_IC : bahn.de
			return Products.INTERCITYUNDEUROCITYZUEGE;
		case "inter_regional_train": // origin: INTER_REGIONAL_TRAIN : bahnhof.de, regio-guide.de
		case "regionalexpress": // origin: regionalExpress :
		case "ir": // origin: IR : bahn.de
			return Products.INTERREGIOUNDSCHNELLZUEGE;
		case "regional_train": // origin: REGIONAL_TRAIN : bahnhof.de, regio-guide.de
		case "regional": // origin: REGIONAL : bahn.de
			return Products.NAHVERKEHRSONSTIGEZUEGE;
		case "city_train": // origin: CITY_TRAIN : bahnhof.de, regio-guide.de
		case "suburban": // origin: suburban :
		case "sbahn": // origin: SBAHN : bahn.de
			return Products.SBAHNEN;
		case "bus": // origin: BUS : bahnhof.de, regio-guide.de
			return Products.BUSSE;
		case "ferry": // origin: FERRY : bahnhof.de, regio-guide.de
		case "schiff": // origin: SCHIFF : bahn.de
			return Products.SCHIFFE;
		case "subway": // origin: SUBWAY : bahnhof.de, regio-guide.de
		case "ubahn": // origin: UBAHN : bahn.de
			return Products.UBAHN;
		case "tram": // origin: TRAM : bahnhof.de, regio-guide.de, bahn.de
			return Products.STRASSENBAHN;
		case "shuttle": // origin: SHUTTLE : bahnhof.de, regio-guide.de
		case "taxi": // origin: TAXI :
		case "anrufpflichtig": // origin: ANRUFPFLICHTIG : bahn.de
			return Products.ANRUFPFLICHTIGEVERKEHRE;
		default:
			return Products.UNKNOWN;
	}
};

export { Products, mapToProduct };
