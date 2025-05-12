import type { DateTime } from "luxon";
import type { ObjectId } from "mongodb";
import type { Station } from "navigator-core/src/models/station";
import type { Connection } from "navigator-core/src/models/connection";

interface StationDocument extends Station {
	_id?: ObjectId;
	queryingEnabled?: boolean;
	lastQueried?: DateTime;
}

interface ConnectionDocument extends Connection {
	_id?: ObjectId;
}

export type { StationDocument, ConnectionDocument };
