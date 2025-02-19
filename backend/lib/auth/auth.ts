import { type ExpressAuthConfig } from "@auth/express";
import GitHub from "@auth/core/providers/github";
import { PrismaAdapter } from "@auth/prisma-adapter";
import { prisma } from "../database/prisma.ts";

declare module "@auth/express" {
	interface User {
		handle: string;
	}
}

declare module "@auth/core/adapters" {
	interface AdapterUser {
		handle: string;
	}
}

const authConfig: ExpressAuthConfig = {
	providers: [
		GitHub({
			clientId: process.env.GITHUB_CLIENT_ID,
			clientSecret: process.env.GITHUB_CLIENT_SECRET,
			profile(profile) {
				return {
					id: profile.id?.toString(),
					name: profile.name,
					handle: profile.login,
					email: profile.email,
					image: profile.avatar_url
				}
			}
		})
	],
	callbacks: {
		session({ session, user }) {
			if (session.user && user) {
				session.user.handle = user.handle; // 'handle' is stored in your DB
			}
			return session;
		}
	},
	adapter: PrismaAdapter(prisma),
	secret: process.env.AUTH_SECRET,
};

export default authConfig;