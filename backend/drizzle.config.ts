import { defineConfig } from "drizzle-kit";

export default defineConfig({
	out: "./drizzle",
	schema: ["./src/db/*.ts"],
	schemaFilter: ["auth", "data"],
	dialect: "postgresql",
	dbCredentials: {
		url: process.env.AUTH_POSTGRES_URL!
	}
});
