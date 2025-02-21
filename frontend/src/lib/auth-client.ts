import { createAuthClient } from "better-auth/svelte";

export const authClient = createAuthClient({
	baseURL: "http://localhost:8000/auth",
});