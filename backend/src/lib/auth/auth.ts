import { prismaAdapter } from "better-auth/adapters/prisma";
import { prisma } from "../database/prisma.ts";
import { betterAuth } from "better-auth";

export const auth = betterAuth({
	database: prismaAdapter(prisma, { provider: "postgresql" }),
	socialProviders: {
		github: {
			enabled: true,
			clientId: process.env.GITHUB_CLIENT_ID!,
			clientSecret: process.env.GITHUB_CLIENT_SECRET!,
		}
	},
	basePath: "/auth",
	trustedOrigins: ["http://localhost:5173"],
	secret: process.env.BETTER_AUTH_SECRET!,
});