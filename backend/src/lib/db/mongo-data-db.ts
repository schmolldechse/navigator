import { Collection, Db, MongoClient } from "mongodb";
import type { ConnectionDocument, StationDocument } from "../../db/mongodb/station.schema.ts";

interface Collections {
	stations: StationDocument;
	trips: ConnectionDocument;
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

export { type StationDocument, connectToDb, getCollection };
