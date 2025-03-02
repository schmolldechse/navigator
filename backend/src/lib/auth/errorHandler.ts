import express, { type NextFunction } from "express";
import { HttpError } from "../errors/HttpError.ts";

export function errorHandler(err: unknown, req: express.Request, res: express.Response, next: NextFunction): void {
	if (err instanceof HttpError) {
		res.status(err.status).json();
		return;
	}

	next();
}
