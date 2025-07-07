import { betterAuth } from "better-auth";
import { usernamePlugin } from "./plugins/username";
import type { GithubProfile } from "better-auth/social-providers";
import { rolePlugin } from "./plugins/role";
import Elysia, { Context } from "elysia";
import { openAPI } from "better-auth/plugins";
import { database } from "../db/postgres";
import { cors } from "@elysiajs/cors";
import { drizzleAdapter } from "better-auth/adapters/drizzle";
import { account, session, user, verification } from "../db/auth.schema";

const auth = betterAuth({
	database: drizzleAdapter(database, {
		provider: "pg",
		schema: {
			user,
			session,
			account,
			verification
		}
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
	plugins: [usernamePlugin(), rolePlugin(), openAPI()]
});

const authApp = new Elysia()
	.use(
		cors({
			origin: ["http://localhost:5173", "http://localhost:3000"],
			credentials: true,
			methods: ["GET", "POST", "PUT", "DELETE", "OPTIONS"],
			allowedHeaders: ["Content-Type", "Authorization", "Cookie"]
		})
	)
	.all("/api/auth/*", (context: Context) => {
		const BETTER_AUTH_ACCEPT_METHODS = ["POST", "GET"];
		// validate request method
		if (BETTER_AUTH_ACCEPT_METHODS.includes(context.request.method)) return auth.handler(context.request);
		else context.status(405);
	});

// OpenAPI schema
let _schema: ReturnType<typeof auth.api.generateOpenAPISchema>;
const getSchema = async () => (_schema ??= auth.api.generateOpenAPISchema());

const OpenAPI = {
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

export { auth, authApp, OpenAPI };
