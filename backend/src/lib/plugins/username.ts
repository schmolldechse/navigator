import type { BetterAuthPlugin } from "better-auth";

export const usernamePlugin = () => {
	return {
		id: "username-plugin",
		schema: {
			user: {
				fields: {
					username: {
						type: "string",
						required: true,
						fieldName: "username",
						unique: true
					}
				}
			}
		}
	} satisfies BetterAuthPlugin;
};