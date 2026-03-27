import { defineConfig } from "@hey-api/openapi-ts";

export default defineConfig({
	input: "http://localhost:5019/swagger.json",
	output: "./src/lib/api",
	parser: {
		transforms: {
			enums: {
				case: "SCREAMING_SNAKE_CASE"
			}
		}
	},
	plugins: [
		{
			name: "@hey-api/typescript",
			enums: {
				mode: "typescript",
				case: "SCREAMING_SNAKE_CASE"
			}
		},
		{
			name: "valibot",
			requests: false,
			responses: false
		}
	]
});
