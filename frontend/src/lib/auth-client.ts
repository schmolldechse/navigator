import { createAuthClient } from "better-auth/svelte";
import { usernameClientPlugin } from "./plugins/username";

export const authClient = createAuthClient({
	baseURL: "http://localhost:8000/auth",
	plugins: [usernameClientPlugin()]
});
