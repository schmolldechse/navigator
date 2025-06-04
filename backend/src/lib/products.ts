import { Product } from "../models/core/models";

const Products = {
	HOCHGESCHWINDIGKEITSZUEGE: {
		value: "HOCHGESCHWINDIGKEITSZUEGE",
		possibilities: ["HIGH_SPEED_TRAIN", "nationalExpress"]
	},
	INTERCITYUNDEUROCITYZUEGE: { value: "INTERCITYUNDEUROCITYZUEGE", possibilities: ["INTERCITY_TRAIN", "national"] },
	INTERREGIOUNDSCHNELLZUEGE: {
		value: "INTERREGIOUNDSCHNELLZUEGE",
		possibilities: ["INTER_REGIONAL_TRAIN", "regionalExpress"]
	},
	NAHVERKEHRSONSTIGEZUEGE: {
		value: "NAHVERKEHRSONSTIGEZUEGE",
		possibilities: ["REGIONAL_TRAIN", "regional"]
	},
	SBAHNEN: { value: "SBAHNEN", possibilities: ["CITY_TRAIN", "suburban"] },
	BUSSE: { value: "BUSSE", possibilities: ["BUS", "bus"] },
	SCHIFFE: { value: "SCHIFFE", possibilities: ["FERRY", "ferry"] },
	UBAHN: { value: "UBAHN", possibilities: ["SUBWAY", "subway"] },
	STRASSENBAHN: { value: "STRASSENBAHN", possibilities: ["TRAM", "tram"] },
	ANRUFPFLICHTIGEVERKEHRE: { value: "ANRUFPFLICHTIGEVERKEHRE", possibilities: ["SHUTTLE", "taxi"] },
	UNKNOWN: { value: "UNKNOWN", possibilities: [] }
} as const;

const mapToProduct = (input?: string): Product => {
	if (!input) return Products.UNKNOWN as unknown as Product;

	const matchedProduct = Object.values(Products).find(
		(product) =>
			product.value.toLowerCase() === input.toLowerCase() ||
			product.possibilities.some((possibility: string) => possibility.toLowerCase() === input.toLowerCase())
	);
	return (matchedProduct || Products.UNKNOWN) as unknown as Product;
};

export { Products, mapToProduct };