import type { BetterAuthClientPlugin, BetterAuthPlugin } from "better-auth";

export const rolePlugin = () => {
	return {
		id: "role-plugin",
		schema: {
			user: {
				fields: {
					role: {
						type: "string",
						fieldName: "role",
						required: true
					}
				}
			}
		}
	} satisfies BetterAuthPlugin;
};

export const roleClientPlugin = () => {
	return {
		id: "role-client-plugin",
		$InferServerPlugin: {} as ReturnType<typeof rolePlugin>
	} satisfies BetterAuthClientPlugin;
};
