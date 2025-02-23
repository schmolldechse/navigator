import type { BetterAuthClientPlugin, BetterAuthPlugin } from "better-auth";

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

export const usernameClientPlugin = () => {
	return {
		id: "username-client-plugin",
		$InferServerPlugin: {} as ReturnType<typeof usernamePlugin>
	} satisfies BetterAuthClientPlugin;
};
