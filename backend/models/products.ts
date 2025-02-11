/**
 * See comments for more possible values.
 * - uppercase -> RIS
 * - lowercase -> HAFAS
 */
enum Products {
	HOCHGESCHWINDIGKEITSZUEGE = "HOCHGESCHWINDIGKEITSZUEGE", // HIGH_SPEED_TRAIN, nationalExpress
	INTERCITYUNDEUROCITYZUEGE = "INTERCITYUNDEUROCITYZUEGE", // INTERCITY_TRAIN, national
	INTERREGIOUNDSCHNELLZUEGE = "INTERREGIOUNDSCHNELLZUEGE", // INTER_REGIONAL_TRAIN
	NAHVERKEHRSONSTIGEZUEGE = "NAHVERKEHRSONSTIGEZUEGE", // REGIONAL_TRAIN, regional
	SBAHNEN = "SBAHNEN", // CITY_TRAIN, suburban
	BUSSE = "BUSSE", // BUS, bus
	SCHIFFE = "SCHIFFE", // FERRY, ferry
	UBAHN = "UBAHN", // SUBWAY, subway
	STRASSENBAHN = "STRASSENBAHN", // TRAM, tram
	ANRUFPFLICHTIGEVERKEHRE = "ANRUFPFLICHTIGEVERKEHRE"
	// UNKNOWN
}

const mapToEnum = (value: string): Products | undefined => {
	return Object.keys(Products).some((key) => Products[key as keyof typeof Products] === value)
		? (value as Products)
		: undefined;
};

export { mapToEnum, Products };
