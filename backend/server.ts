// @ts-types="npm:@types/express"
import express from "npm:express";
import cors from "npm:cors";
import router from "./router.ts";

const app = express();

app.use(express.json());
app.use(cors("*"));
app.use(router);

app.listen(8000, () => console.log("Server is running on port 8000"));