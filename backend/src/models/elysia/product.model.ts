import { t } from "elysia";

const ProductSchema = t.Object(
	{
		value: t.String({ description: "The name of the product, obtained from Vendo DB API." }),
		possibilities: t.Array(t.String(), { description: "An array of product names that can be matched to the value." })
	},
	{ description: "A Product represents a specific type of transportation." }
);

export { ProductSchema };