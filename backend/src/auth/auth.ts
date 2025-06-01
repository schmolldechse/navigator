import { betterAuth } from "better-auth";
import { usernamePlugin } from "./plugins/username";
import type { GithubProfile } from "better-auth/social-providers";
import { Pool } from "pg";
import { rolePlugin } from "./plugins/role";
import Elysia, { Context, error } from "elysia";
import { openAPI } from "better-auth/plugins";

const auth = betterAuth({
	database: new Pool({
		connectionString: process.env.POSTGRES_CONNECTION_STRING!,
	}),
	socialProviders: {
		github: {
			enabled: true,
			clientId: process.env.GITHUB_CLIENT_ID!,
			clientSecret: process.env.GITHUB_CLIENT_SECRET!,
			mapProfileToUser: (profile: GithubProfile) => ({ username: profile.login })
		}
	},
	basePath: "/api/auth",
	trustedOrigins: ["http://localhost:5173"],
	secret: process.env.AUTH_SECRET!,

	// TODO: extract plugins into a separate repo
	plugins: [usernamePlugin(), rolePlugin(), openAPI()]
});

const authApp = new Elysia().all("/api/auth/*", (context: Context) => {
	const BETTER_AUTH_ACCEPT_METHODS = ["POST", "GET"];
	// validate request method
	if (BETTER_AUTH_ACCEPT_METHODS.includes(context.request.method)) return auth.handler(context.request);
	else error(405, "Method Not Allowed");
});

export { auth, authApp };
