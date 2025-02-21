import cors from "cors";
import swaggerUi from "swagger-ui-express";
import swaggerDocument from "../build/swagger.json";
import { RegisterRoutes } from "../build/routes.ts";
import express from "express";
import * as path from "node:path";
import { toNodeHandler } from "better-auth/node";
import { auth } from "./lib/auth/auth.ts";

const app = express();

app.all("/auth/*", toNodeHandler(auth));

app.use(express.json());
app.use(cors({ origin: "*" }));

app.use("/api-docs", swaggerUi.serve, swaggerUi.setup(swaggerDocument));
app.get("/api-spec", (req, res) => res.sendFile(path.join(__dirname, "build", "swagger.json")));
app.get("/api", (req, res) => res.redirect("/api-docs"));

RegisterRoutes(app);

app.listen(8000, () => console.log("Server is running on port 8000"));
