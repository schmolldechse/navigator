import type { DateTime } from "luxon";
import type { Station } from "../../models/station.ts";
import type { ObjectId } from "mongodb";
import type { Connection } from "../../models/connection.ts";

interface StationDocument extends Station {
	_id?: ObjectId;
	queryingEnabled?: boolean;
	lastQueried?: DateTime;
}

interface ConnectionDocument extends Connection {
	_id?: ObjectId;
}

export type { StationDocument, ConnectionDocument };
