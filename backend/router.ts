// @ts-types="npm:@types/express"
import express from "npm:express";
import { StationController } from "./controllers/stations.ts";
import { TimetableController } from "./controllers/timetable/timetable.ts";

const router = express.Router();

router.use(new StationController().router);
router.use(new TimetableController().router);

export default router;
