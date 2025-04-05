import type { StationDocument } from "../../db/mongodb/station.schema.ts";
import { getCollection } from "../../lib/db/mongo-data-db.ts";

const getCachedStation = async (evaNumber: number): Promise<StationDocument | null> => {
	const collection = await getCollection("stations");
	return (await collection.findOne({ evaNumber })) as StationDocument;
};

export { getCachedStation };