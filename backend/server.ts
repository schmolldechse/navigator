import cors from "cors";
import swaggerUi from "swagger-ui-express";
import swaggerDocument from "./build/swagger.json";
import { RegisterRoutes } from "./build/routes.ts";
import express from "express";
import * as path from "node:path";
import { ExpressAuth } from "@auth/express";
import authConfig from "./lib/auth/auth.ts";

const app = express();

app.use(express.json());
app.use(cors({ origin: "*" }));

app.use("/api-docs", swaggerUi.serve, swaggerUi.setup(swaggerDocument));
app.get("/api-spec", (req, res) => res.sendFile(path.join(__dirname, "build", "swagger.json")));
app.get("/api", (req, res) => res.redirect("/api-docs"));

RegisterRoutes(app);

app.use("/auth", ExpressAuth(authConfig));

app.listen(8000, () => console.log("Server is running on port 8000"));
