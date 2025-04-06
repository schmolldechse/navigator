/**
 * A Product represents a specific type of transportation.
 * It is used by both {@link Connection} and {@link Station} to specify the type of transportation.
 *
 * The Product consists of:
 * - value: The name of the product obtained from the Vendo DeutscheBahn API
 * - possibilities: An array of product names that can be matched to the value. For example, if the API returns "HIGH_SPEED_TRAIN", it maps to HOCHGESCHWINDIGKEITSZUEGE
 */
interface Product {
	value: string;
	possibilities: string[];
}

/**
 * A collection of Products that map a specific transportation category to its corresponding value
 */
const Products = {
	HOCHGESCHWINDIGKEITSZUEGE: {
		value: "HOCHGESCHWINDIGKEITSZUEGE",
		possibilities: ["HIGH_SPEED_TRAIN", "nationalExpress"]
	},
	INTERCITYUNDEUROCITYZUEGE: { value: "INTERCITYUNDEUROCITYZUEGE", possibilities: ["INTERCITY_TRAIN", "national"] },
	INTERREGIOUNDSCHNELLZUEGE: { value: "INTERREGIOUNDSCHNELLZUEGE", possibilities: ["INTER_REGIONAL_TRAIN"] },
	NAHVERKEHRSONSTIGEZUEGE: {
		value: "NAHVERKEHRSONSTIGEZUEGE",
		possibilities: ["REGIONAL_TRAIN", "regionalExpress", "regional"]
	},
	SBAHNEN: { value: "SBAHNEN", possibilities: ["CITY_TRAIN", "suburban"] },
	BUSSE: { value: "BUSSE", possibilities: ["BUS"] },
	SCHIFFE: { value: "SCHIFFE", possibilities: ["FERRY"] },
	UBAHN: { value: "UBAHN", possibilities: ["SUBWAY"] },
	STRASSENBAHN: { value: "STRASSENBAHN", possibilities: ["TRAM"] },
	ANRUFPFLICHTIGEVERKEHRE: { value: "ANRUFPFLICHTIGEVERKEHRE", possibilities: ["SHUTTLE", "taxi"] },
	UNKNOWN: { value: "UNKNOWN", possibilities: [] }
} as const;

type ProductType = (typeof Products)[keyof typeof Products];
const mapToProduct = (input?: string): ProductType => {
	if (!input) return Products.UNKNOWN;

	const matchedProduct = Object.values(Products).find(
		(product) =>
			product.value.toLowerCase() === input.toLowerCase() ||
			product.possibilities.some((possibility) => possibility.toLowerCase() === input.toLowerCase())
	);
	return matchedProduct || Products.UNKNOWN;
};

export { type Product, Products, mapToProduct };
