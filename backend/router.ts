// @ts-types="npm:@types/express"
import express from "npm:express";
import {StationController} from "./controllers/stations.ts";

const router = express.Router();

const stationController = new StationController();
router.use(stationController.router);

export default router;