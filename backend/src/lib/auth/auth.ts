import { betterAuth } from "better-auth";
import { usernamePlugin } from "../plugins/username.ts";
import type { GithubProfile } from "better-auth/social-providers";
import { Pool } from "pg";

export const auth = betterAuth({
	database: new Pool({
		connectionString: process.env.AUTH_POSTGRES_URL!,
	}),
	socialProviders: {
		github: {
			enabled: true,
			clientId: process.env.GITHUB_CLIENT_ID!,
			clientSecret: process.env.GITHUB_CLIENT_SECRET!,
			mapProfileToUser: (profile: GithubProfile) => ({ username: profile.login })
		}
	},
	session: {
		cookieCache: {
			enabled: true,
			maxAge: 5 * 60
		}
	},
	basePath: "/auth",
	trustedOrigins: ["http://localhost:5173"],
	secret: process.env.AUTH_SECRET!,
	plugins: [usernamePlugin()]
});
