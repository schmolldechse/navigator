// See https://svelte.dev/docs/kit/types#app.d.ts
// for information about these interfaces
import { Session, User } from "better-auth";

interface UserWithRole extends User {
	role?: string;
}

declare global {
	namespace App {
		// interface Error {}
		interface Locals {
			session: Session | undefined;
			user: UserWithRole | undefined;
		}
		// interface PageData {}
		// interface PageState {}
		// interface Platform {}
	}
}

export {};
