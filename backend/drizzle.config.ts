import { defineConfig } from "drizzle-kit";

export default defineConfig({
	out: "./drizzle",
	schema: ["./src/db/*.schema.ts"],
	schemaFilter: ["auth", "data"],
	dialect: "postgresql",
	dbCredentials: {
		url: process.env.POSTGRES_CONNECTION_STRING!
	}
});
