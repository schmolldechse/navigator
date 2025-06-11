import { betterAuth } from "better-auth";
import { usernamePlugin } from "./plugins/username";
import type { GithubProfile } from "better-auth/social-providers";
import { rolePlugin } from "./plugins/role";
import Elysia, { Context, error } from "elysia";
import { openAPI } from "better-auth/plugins";
import { pool } from "../db/postgres";

const auth = betterAuth({
	database: pool,
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

// elysia application
const authApp = new Elysia().all("/api/auth/*", (context: Context) => {
	const BETTER_AUTH_ACCEPT_METHODS = ["POST", "GET"];
	// validate request method
	if (BETTER_AUTH_ACCEPT_METHODS.includes(context.request.method)) return auth.handler(context.request);
	else error(405, "Method Not Allowed");
});

// OpenAPI schema
let _schema: ReturnType<typeof auth.api.generateOpenAPISchema>;
const getSchema = async () => (_schema ??= auth.api.generateOpenAPISchema());

const AuthOpenAPI = {
	getPaths: (prefix = "/api/auth") =>
		getSchema().then(({ paths }) => {
			const reference: typeof paths = Object.create(null);

			for (const path of Object.keys(paths)) {
				const key = prefix + path;
				reference[key] = paths[path];

				for (const method of Object.keys(paths[path])) {
					const operation = (reference[key] as any)[method];
					operation.tags = ["Auth"];
				}
			}

			return reference;
		}) as Promise<any>,
	components: getSchema().then(({ components }) => components) as Promise<any>
} as const;

export { auth, authApp, AuthOpenAPI };
