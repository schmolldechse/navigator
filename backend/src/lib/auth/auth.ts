import { betterAuth } from "better-auth";
import { usernamePlugin } from "../plugins/username.ts";
import type { GithubProfile } from "better-auth/social-providers";
import { Pool } from "pg";
import { rolePlugin } from "../plugins/role.ts";
import { openAPI } from "better-auth/plugins";

export const auth = betterAuth({
	database: new Pool({
		connectionString: process.env.AUTH_POSTGRES_URL! + "?options=-csearch_path=auth"
	}),
	socialProviders: {
		github: {
			enabled: true,
			clientId: process.env.GITHUB_CLIENT_ID!,
			clientSecret: process.env.GITHUB_CLIENT_SECRET!,
			mapProfileToUser: (profile: GithubProfile) => ({ username: profile.login })
		}
	},
	basePath: "/auth",
	trustedOrigins: ["http://localhost:5173"],
	secret: process.env.AUTH_SECRET!,

	// TODO: extract plugins into a separate repo
	plugins: [usernamePlugin(), rolePlugin(), openAPI()]
});
