import { defineConfig } from "@hey-api/openapi-ts";

export default defineConfig({
	input: "https://navigator.voldechse.wtf/swagger.json",
	output: "./src/lib/api",
	plugins: [
		{
			name: "@hey-api/typescript",
			enums: {
				mode: "typescript",
				case: "SCREAMING_SNAKE_CASE"
			}
		}
	]
});
