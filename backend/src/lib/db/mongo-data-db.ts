import { Collection, Db, MongoClient } from "mongodb";
import type { StationDocument } from "../../db/mongodb/station.schema.ts";

export interface Collections {
	stations: StationDocument;
}

let cachedDb: Db | null = null;

const connectToDb = async (): Promise<Db> => {
	if (cachedDb) return cachedDb;

	const client = await MongoClient.connect(process.env.MONGO_URL!);
	cachedDb = client.db("data");

	return cachedDb;
};

const getCollection = async <T extends keyof Collections>(name: T): Promise<Collection<Collections[T]>> => {
	const db = await connectToDb();
	return db.collection<Collections[T]>(name);
};

export { connectToDb, getCollection };