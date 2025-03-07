import type { DateTime } from "luxon";
import type { Station } from "../../models/station.ts";
import type { ObjectId } from "mongodb";

interface StationDocument extends Station {
	_id?: ObjectId;
	lastQueried?: DateTime;
}

export type { StationDocument };