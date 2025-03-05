import { createAuthClient } from "better-auth/svelte";
import { usernameClientPlugin } from "./plugins/username";
import { roleClientPlugin } from "./plugins/role";
import { env } from "$env/dynamic/public";

export const authClient = createAuthClient({
	baseURL: `${env.PUBLIC_BACKEND_BASE_URL}/auth`,
	plugins: [usernameClientPlugin(), roleClientPlugin()]
});
