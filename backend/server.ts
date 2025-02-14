import cors from "cors";
import swaggerUi from "swagger-ui-express";
import swaggerDocument from "./build/swagger.json";
import { RegisterRoutes } from "./build/routes.ts";
import express from "express";

const app = express();

app.use(express.json());
app.use(cors({ origin: "*" }));

app.use("/api-docs", swaggerUi.serve, swaggerUi.setup(swaggerDocument));
app.get("/api", (req, res) => res.redirect("/api-docs"));

RegisterRoutes(app);

app.listen(8000, () => console.log("Server is running on port 8000"));
