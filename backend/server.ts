import express from "express";
import cors from "cors";
import swaggerUi from "swagger-ui-express";

const app = express();

import swaggerDocument from "./build/swagger.json";
import { RegisterRoutes } from "./build/routes.ts";

app.use(express.json());
app.use(cors({ origin: "*" }));

app.use("/api-docs", swaggerUi.serve, swaggerUi.setup(swaggerDocument));

RegisterRoutes(app);

app.listen(8000, () => console.log("Server is running on port 8000"));
