// @ts-types="npm:@types/express"
import express from "npm:express";
import { StationController } from "./controllers/stations.ts";
import { BahnhofController } from "./controllers/bahnhof.ts";

const router = express.Router();

router.use(new StationController().router);
router.use(new BahnhofController().router);

export default router;
