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
	ANRUFPFLICHTIGEVERKEHRE = "ANRUFPFLICHTIGEVERKEHRE",
	UNKNOWN = "UNKNOWN"
}

const keyValueMap: Record<string, Products> = {
	highspeedtrain: Products.HOCHGESCHWINDIGKEITSZUEGE,
	nationalexpress: Products.HOCHGESCHWINDIGKEITSZUEGE,

	intercitytrain: Products.INTERCITYUNDEUROCITYZUEGE,
	national: Products.INTERREGIOUNDSCHNELLZUEGE,

	interregionaltrain: Products.INTERREGIOUNDSCHNELLZUEGE,

	regionaltrain: Products.NAHVERKEHRSONSTIGEZUEGE,
	regionalexpress: Products.NAHVERKEHRSONSTIGEZUEGE,
	regional: Products.NAHVERKEHRSONSTIGEZUEGE,

	citytrain: Products.SBAHNEN,
	suburban: Products.SBAHNEN,

	bus: Products.BUSSE,

	ferry: Products.SCHIFFE,

	subway: Products.UBAHN,

	tram: Products.STRASSENBAHN
};

const mapToProduct = (input: string | undefined): Products => {
	if (!input) return Products.UNKNOWN;

	const key = input.replace(/[_-]/g, "").toLowerCase();
	return keyValueMap[key] ?? Products.UNKNOWN;
};

const mapToEnum = (value: string): Products | undefined => {
	return Object.keys(Products).some((key) => Products[key as keyof typeof Products] === value)
		? (value as Products)
		: undefined;
};

export { mapToEnum, mapToProduct, Products };
