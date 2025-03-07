import express from "express";
import { auth } from "./auth.ts";
import { HttpError } from "../errors/HttpError.ts";

export const expressAuthentication = async (req: express.Request, securityName: string, scopes?: string[]) => {
	if (securityName === "better_auth") {
		const session = await auth.api.getSession({ headers: new Headers(req.headers as Record<string, string>) });
		if (!session?.user) {
			return Promise.reject(new HttpError(401, "Unauthorized"));
		}

		if (scopes?.includes("admin") && session?.user?.role !== "admin") {
			return Promise.reject(new HttpError(401, "Unauthorized"));
		}

		return Promise.resolve();
	}
};
