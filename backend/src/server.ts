import cors from "cors";
import { RegisterRoutes } from "../build/routes.ts";
import express from "express";
import * as path from "node:path";
import { toNodeHandler } from "better-auth/node";
import { auth } from "./lib/auth/auth.ts";
import { errorHandler } from "./lib/auth/errorHandler.ts";
import { apiReference } from "@scalar/express-api-reference";

const app = express();

app.all("/auth/*", toNodeHandler(auth));

app.use(express.json());
app.use(cors({ origin: "*" }));

app.get("/openapi.json", (req, res) => res.sendFile(path.join(__dirname, "..", "build", "openapi.json")));
app.use(
	"/api-docs",
	apiReference({
		theme: "purple",
		spec: {
			url: "/openapi.json"
		}
	})
);
app.get("/api", (req, res) => res.redirect("/api-docs"));

RegisterRoutes(app);

app.use(errorHandler);

app.listen(8000, () => console.log("Server is running on port 8000"));
