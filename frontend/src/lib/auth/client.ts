import { createAuthClient } from "better-auth/svelte";
import { usernameClientPlugin } from "$lib/auth/plugins/username";
import { roleClientPlugin } from "$lib/auth/plugins/role";
import { env } from "$env/dynamic/public";

export const client = createAuthClient({
	baseURL: `${env.PUBLIC_BACKEND_URL}/api/auth`,
	plugins: [usernameClientPlugin(), roleClientPlugin()]
});
